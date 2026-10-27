# Alignator-4 — Align Records (`with` expressions): Reference

## Goal

Align the property assignments of a C# `with` expression initializer by padding the
whitespace before the `=`, so the equals signs — and with them the values — share one
column when the assignments are written on separate lines.

One new setting:

| Setting | Values | EditorConfig | Default |
|---|---|---|---|
| Align assignments in with expression | `true`, `false` | `csharp_alignator_align_assignments_in_with_expression` | `false` |

Behavior contracts (from the issue):

1. Padding only — never moves an assignment onto its own line.
2. Single-line `with` expressions stay untouched.
3. Every `with` expression aligns independently (no cross-alignment between groups).
4. Defaults leave code byte-identical (no Alignator effect).

## Acceptance criteria

1. The three outputs in the issue ("Expected" section, independence example, single-line
   guard) are reproduced by automated formatter tests (`.cs`/`.cs.gold`).
2. Default (`false`) leaves code byte-identical.
3. Setting configurable via .editorconfig: `csharp_alignator_align_assignments_in_with_expression = true|false`.
4. Setting appears in Settings → Code Editing → C# → Code Style → Others → Alignator page,
   with Jurassic Park dinosaur preview code (AGENTS.md Rule 13).
5. Existing rules and tests are unaffected (object initializers `new() { Height = 2 }`
   are `IMemberInitializer`, a different node type — untouched).

## Relevant files and symbols

| File | Symbols |
|------|---------|
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProvider.cs` | `AlignArgumentsProvider.Initialize()`; `INT_ALIGN_ARGUMENT_NAMES_AfterNameColon` (pattern for per-node offset keys and line-break guards); `INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER_BeforeClosingBrace` (pattern for a bool setting + shared key per scope); helpers `EntryStartsOwnLine`, `ContainsLineBreak`, `PreviousNode`, `NextNode` — all reusable unchanged |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs` | `AlignatorSettingsKey` — settings fields with `[EditorConfigEntryAlias]`; class-level `[EditorConfigKey("csharp")]` supplies the `csharp_` prefix of every alias |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs` | `DescribePart` — `builder.ItemFor(...)` with preview snippet, `parseType: PreviewParseType.Member`, `previewFixup: IntAlignFixup` |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/CSharpQuirkyFormattingTest.cs` | Test class: `[TestAlignator]`, class-level `[TestSetting]`s (`WRAP_LINES=false`, `INT_ALIGN_INVOCATIONS=true`), per-test `[TestSetting]` + `DoNamedTest2()` |
| `src/dotnet/Alignator.Tests/test/data/CodeFormatter/CSharp/AlignArguments/` | `.cs` / `.cs.gold` pairs, 2-space indent, `seealso` doc comment referencing the exercised setting |
| `src/dotnet/Alignator.Tests/Data/TestData.cs` | Dummy types for test data — needs new record stubs (see below) |
| `AGENTS.md` | Rules/settings tables — update after implementation (Alignator-3 precedent: only AGENTS.md, not README/CHANGELOG) |

## How the formatter integration works (from the existing rules)

- Rules are `IntAlignRule`s registered via `DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()`
  in `Initialize()`, priority `100_000` (`AlignatorPriority`).
- A rule matches a whitespace *gap* through `Where(Left(), Right(), Parent())` and returns an
  `IntAlignOptionValue(key, priority)` from `When(value).Calculate(...)`. All gaps sharing the
  same key are padded to a common column; gaps returning `null` are untouched. A key held by
  a single gap means no visible change (this is what keeps single-line constructs untouched).
- `.SwitchOnExternalKey` + `When(true)` binds the rule to the new bool setting, default `false`.
- Alignment group = unique part of the key. Per-call rules use the enclosing node's document
  start offset; here: one shared key per `with` expression, without an element index (each
  line has exactly one `=` gap — same scheme as the dictionary closing-brace rule).
- All Alignator rules only take effect when the built-in `int_align_variables`
  (`INT_ALIGN_INVOCATIONS` in tests) is enabled — test class already sets it.

## PSI structure for `with` expressions (VERIFIED, see psi-dump.txt)

For `x with { Name = "Echo", Speed = 60 }` the R# 2025.3 PSI tree is:

```
WithExpression (IWithExpression)
├─ <operand expression>
├─ WITH_KEYWORD
├─ LBRACE '{'
├─ [per assignment:] WithInitializerAssignment (IWithInitializerAssignment)
│    ├─ NameIdentifier (ICSharpIdentifier)          ← property name
│    ├─ Whitespace                                   ← the gap this feature pads
│    ├─ Eq token (CSharpTokenType.EQ)
│    ├─ Whitespace
│    └─ <value expression>
├─ CommaTokenElement COMMA                           ← separators are children of
├─ NewLine, Whitespace                                  IWithExpression
└─ RBRACE '}'
```

Facts that drive the rule predicates:

- Assignments are `IWithInitializerAssignment` — NOT `ICSharpArgument`, NOT
  `IMemberInitializer` (object initializers). `Parent().Satisfies(node is IWithInitializerAssignment)`
  is therefore precise and excludes every other `=` gap in the file.
- The gap to pad sits between the `NameIdentifier` and the `Eq` token; both are direct
  children of the assignment node.
- Walk-back from the first assignment reaches `LBRACE`; from later assignments the
  separating `COMMA`; whitespace with `\n` marks own-line starts — exactly the token
  set `EntryStartsOwnLine` already switches on (LBRACE/COMMA/newline). Reusable as is.
- Unverified detail: whether `Left()` receives the `ICSharpIdentifier` wrapper or its
  name token. If the first rule attempt stays RED, dump the runtime gap nodes
  (temporary debug in a test, like Alignator-3's psi-dump step) and adjust the
  `Left()` predicate accordingly.

## Test data stubs (TestData.cs, 4-space indent, Jurassic Park Rule 13)

```cs
internal enum DangerLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Extreme = 4,
}

internal record DinosaurRecord(string Name, string Diet, int PaddockNumber, DangerLevel DangerLevel, bool ThermalCamouflageEnabled, int Speed = 0);

internal record FeedingSchedule(string Time, string Food, int Quantity, string Recipient);

internal sealed class ParkSettings
{
    public static ParkSettings Instance { get; } = new();
    private ParkSettings() { }
}

internal static class RecordFactory
{
    public static DinosaurRecord GetDinosaurRecord(object source) => new("Echo", "Carnivore", 0, DangerLevel.Low, false);
    public static FeedingSchedule GetFeedingSchedule(object source) => new("09:00", "Goat", 1, "T-Rex");
}
```

`GetDinosaurRecord` takes `object` so both call shapes from the issue compile
(`GetDinosaurRecord(ParkSettings.Instance)` and `GetDinosaurRecord("Echo")`).
Records are C# 9 syntax; `LangVersion=Latest` is set, records work on net472.

## Constraints, conventions and gotchas

- Gold derivation is mechanical: pad before `=` so every `=` of one `with` initializer
  sits at column `indent + max(nameLength) + 1` — the longest name keeps exactly one
  space. Never copy formatter output into a gold (Rule 9).
- Test data files use 2-space indent (test `.editorconfig`); source files use 4-space.
- All example values use Jurassic Park dinosaur names (Rule 13).
- Comma alignment rules and the argument-names rule must not interfere: with-expression
  `=` gaps have no `IArgumentList`/`IAttribute` parent, so their walks return `null`
  already; the new rule requires `IWithInitializerAssignment` parents, so it cannot
  swallow attribute `Name = value` or named-argument gaps. No changes to existing rules.
- Built-in C# formatter may normalize "space before equals": Alignator rules already win
  the same way for space-before-comma (priority `100_000` runs after standard formatting);
  treat as low risk, verify in RED→GREEN.

## Open questions / risks

1. Gap node granularity (identifier wrapper vs. token) — see PSI section; resolve by
   runtime dump if RED persists.
2. Partially-inline initializers (`with { Name = "x",` on the brace line, following
   assignments on own lines): the first assignment opens the list and gets the shared
   key, so it aligns with the own-line assignments. Same semantics as the existing
   dictionary rules; no issue statement contradicts it. Not pinned by a test.
3. README.md settings table is stale since Alignator-3 (dictionary settings missing);
   CHANGELOG.md was not updated for Alignator-3 either. Following that precedent,
   Alignator-4 updates AGENTS.md only. Change requires a separate decision.
