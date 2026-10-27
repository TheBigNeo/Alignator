# Alignator-4 — Align Records (`with` expressions): Plan

Issue #4. Align the property assignments of a `with` expression initializer by padding
before the `=`, one new bool setting, default off.

Approach: one new `IntAlignRule` in `AlignArgumentsProvider` matching the whitespace gap
between an `IWithInitializerAssignment`'s `NameIdentifier` and its `Eq` token, scoped per
`with` expression by the `IWithExpression` start offset. One setting in
`AlignatorSettingsKey`, one UI item in `AlignatorSettingsSchema`, NUnit formatter tests
with `.cs`/`.cs.gold` data. No changes to existing rules.

**TDD:** setting first (compile only), then the full planned test set goes in and runs RED
(guard tests pass trivially, behavior tests fail because no rule exists yet), then the rule
is implemented until the tests are GREEN. Golds encode the alignment spec (shared column =
`indent + max(nameLength) + 1`), derived mechanically — never copied from formatter output.

## Planned test cases

All in `CSharpQuirkyFormattingTest.cs`, data files in
`test/data/CodeFormatter/CSharp/AlignArguments/` (2-space indent, method-local
`with` expressions over the new `DinosaurRecord` / `FeedingSchedule` stubs).

| # | Test method | `[TestSetting]`s | Data files | Expected |
|---|-------------|------------------|------------|----------|
| 1 | `TestAlignAssignmentsInWithExpression` | `INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION = true` | `AlignAssignmentsInWithExpression.cs/.gold` | `=` of all five assignments share one column (issue "Expected") |
| 2 | `TestAlignAssignmentsInWithExpression_IndependentGroups` | `... = true` | `...IndependentGroups.cs/.gold` | Two `with` expressions in one file align independently; the shorter group does not stretch the first |
| 3 | `TestAlignAssignmentsInWithExpression_SingleLineUntouched` | `... = true` | `...SingleLineUntouched.cs/.gold` | Single-line `with` unchanged (guard) |
| 4 | `TestAlignAssignmentsInWithExpression_DoNotChange` | none (defaults) | `...DoNotChange.cs/.gold` | Output byte-identical to input (default = no-op guard) |

Input block (tests 1, 2 first half and 4, identical modulo class name and `seealso`
line):

```cs
namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION"/>
internal class <ClassName>
{
  static void IndominusRex()
  {
    DinosaurRecord indominusRex = GetDinosaurRecord(ParkSettings.Instance)
        with
        {
            Name = "Indominus Rex",
            Diet = "Carnivore",
            PaddockNumber = 11,
            DangerLevel = DangerLevel.Extreme,
            ThermalCamouflageEnabled = true,
        };
  }
}
```

Gold derivation (mechanical): pad before `=` so every `=` of one `with` initializer sits
at `nameIndent + max(nameLength) + 1` columns; the longest name keeps exactly one space.

- Test 1 gold (names padded to 25 columns before `=`):
  `Name` + 21 spaces, `Diet` + 21, `PaddockNumber` + 12, `DangerLevel` + 14,
  `ThermalCamouflageEnabled` + 1 — matching the issue's "Expected" block.
- Test 2 adds a second statement (same method) after the first:
  `FeedingSchedule feedingSchedule = GetFeedingSchedule(ParkSettings.Instance) with { ... }`
  with entries `Time`, `Food`, `Quantity`, `Recipient`; its gold pads names to 10 columns
  (`Time` + 6, `Food` + 6, `Quantity` + 2, `Recipient` + 1), while the first group stays
  padded to 25 — proving independent alignment. See issue's "More test data" block.
- Test 3 input: `DinosaurRecord echo = GetDinosaurRecord("Echo") with { Name = "Echo", Speed = 60 };` — gold == input.
- Test 4: gold == input.

Test stubs (tests reference them, so they land with the RED tests):
`DangerLevel` enum, `DinosaurRecord` and `FeedingSchedule` records, `ParkSettings.Instance`,
`RecordFactory.GetDinosaurRecord/GetFeedingSchedule` — exact code in `reference.md`
("Test data stubs").

## Step Plan

| Status | Step | Verification |
|--------|------|--------------|
| [ ] | 1. Setting: add `INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION` (`bool`, default `false`, alias `alignator_align_assignments_in_with_expression`) to `AlignatorSettingsKey.cs` | Test project compiles: `dotnet build src/dotnet/Alignator.Tests/Alignator.Tests.csproj` |
| [ ] | 2. Tests first: stubs in `TestData.cs`, 4 test cases (data files `.cs` + spec-derived `.gold`, test methods in `CSharpQuirkyFormattingTest.cs`) | `dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj --filter "FullyQualifiedName~CSharpQuirkyFormattingTest"`: tests 1–2 RED (no alignment yet), tests 3–4 pass trivially |
| [ ] | 3. Rule: `INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION_BeforeEq()` in `AlignArgumentsProvider.cs`, registered in `Initialize()`; reuse `EntryStartsOwnLine` / `ContainsLineBreak` unchanged | Tests 1–2 GREEN, tests 3–4 and all 21 existing tests still GREEN |
| [ ] | 4. Settings UI: one `ItemFor` entry in `AlignatorSettingsSchema.DescribePart` (Jurassic Park `with`-expression preview) | `./gradlew buildPlugin` succeeds; visual check in `runIde` optional |
| [ ] | 5. Docs + full gate: update AGENTS.md (settings 7→8, rules 13→14, editorconfig block, key files); run full suite and lint | `./gradlew testDotNet` green, `skirnir-lint` clean, AGENTS.md current |

## Step details

### 1. Setting — `AlignatorSettingsKey.cs`

Follow the existing pattern exactly:

```csharp
[EditorConfigEntryAlias("alignator_align_assignments_in_with_expression", EditorConfigAliasType.LanguageSpecificStandard)]
[SettingsEntry(false, "Align assignments in with expression")]
public bool INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION;
```

The class-level `[EditorConfigKey("csharp")]` supplies the `csharp_` prefix; the full
key then matches the issue's table. No changes to `AlignPosition.cs`.

### 2. Tests first (RED)

- Data files per the table above; inputs identical modulo class name, golds derived
  mechanically per the derivation rule.
- Test methods with `[TestSetting]`, named `Test<file base name>`, `DoNamedTest2()`.
- Test 4 carries no `AlignatorSettingsKey` setting — class-level defaults apply.
- Red check: the formatter output for tests 1–2 equals the input (no rule exists), so
  the gold comparison must fail. Tests 3–4 (gold == input) pass already — they are
  regression guards for the default-off and single-line contracts.

### 3. Rule — `AlignArgumentsProvider.cs`

One rule, modeled on `INT_ALIGN_ARGUMENT_NAMES_AfterNameColon` (per-node offset key,
line-break guards) and `INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER_BeforeClosingBrace`
(bool setting, one shared key per scope):

```csharp
private void INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION_BeforeEq()
{
    DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
       .Name(nameof(AlignatorSettingsKey.INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION) + "_BeforeEq")
       .Where(
            Left().Satisfies((node, _) => node.NodeOrNull is ICSharpIdentifier),
            Right().Satisfies((node, _) => node.NodeOrNull is ITokenNode token
                                           && token.GetTokenType() == CSharpTokenType.EQ),
            Parent().Satisfies((node, _) => node.NodeOrNull is IWithInitializerAssignment)
        )
       .SwitchOnExternalKey(
            (AlignatorSettingsKey x) => x.INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION,
            When(true).Calculate((formattingRangeContext, _) =>
                {
                    // ... see below
                }
            )
        )
       .Build();
}
```

Calculate body (guards in the order used by the dictionary rules):

1. Cast `ctx.Parent.NodeOrNull` to `IWithInitializerAssignment assignment`, else `null`.
2. Walk up from the assignment to `IWithExpression`, else `null` — this rejects every
   other `=` gap and scopes the group.
3. Own-line guard: `if (!EntryStartsOwnLine(assignment)) return null;` — reusable
   unchanged: walk-back hits `LBRACE` (first assignment), `COMMA` (mid-line), or a
   newline. Single-line `with` expressions end up with a single gap per key, and the
   engine no-ops on single-gap keys — contract 2 needs no extra code.
4. Single-line-assignment guard: `if (ContainsLineBreak(assignment)) return null;`.
5. Return `new IntAlignOptionValue($"WithExpr$BeforeEq$With{withExpression.GetDocumentStartOffset().Offset}", AlignatorPriority)` —
   one shared key per `with` expression, no element index (each line has exactly one
   `=` gap): every `=` aligns to one common column, and separate `with` expressions
   never share a key — contract 3.

Register the call in `Initialize()` (last line, after the closing-braces rule).

Contingency (from `reference.md`, open question 1): if tests stay RED after this
implementation, dump the runtime gap nodes — temporary debug output from a formatter
run over test 1's input, delete afterwards — and adjust the `Left()` predicate
(identifier wrapper vs. name token) per the dump.

### 4. Settings UI — `AlignatorSettingsSchema.cs`

One `ItemFor` after the closing-braces item, preview with `parseType:
PreviewParseType.Member`, `previewFixup: IntAlignFixup`:

```
static void IndominusRex()
{
  DinosaurRecord indominusRex = GetDinosaurRecord(ParkSettings.Instance)
      with
      {
        Name = "Indominus Rex",
        Diet = "Carnivore",
        PaddockNumber = 11,
        DangerLevel = DangerLevel.Extreme,
        ThermalCamouflageEnabled = true,
      };
}
```

### 5. Docs + full gate — `AGENTS.md`

- Purpose paragraph: add `with`-expression assignments to the alignment list.
- Alignment rules table: row `With-expression assignments | INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION | IWithInitializerAssignment | Per with expression`.
- "Thirteen rules" wording → fourteen; settings count seven → eight; Configuration and
  .editorconfig blocks gain the new key; Key Files note stays.
- Doc scope decision (Rule 7, visible): Alignator-3 updated AGENTS.md only — README.md
  is stale since then and CHANGELOG.md has no Alignator-3 entry. Alignator-4 follows that
  precedent and updates AGENTS.md only.
- Full gate: `./gradlew testDotNet`, then `skirnir-lint`.

## Notes for execution (PlanRules Phase 4)

- One step at a time; verify each step before the next; mark `[~]`/`[x]` in this table.
- Commit between steps (Rule 10), message prefix `Alignator-4:`.
- Known unpinned edge (reference.md, open question 2): first assignment on the brace
  line aligns with own-line assignments. If a reviewer wants it pinned, add a fifth
  test case in step 2 — do not decide ad hoc in step 3.
