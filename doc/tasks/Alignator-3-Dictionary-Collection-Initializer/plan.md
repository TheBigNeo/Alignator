# Alignator-3 — Dictionary Collection Initializer: Plan

Issue #3. Align `{"Key", Value}` dictionary-style entries of collection initializers
(key→value gap and entry closing brace), two new settings, defaults off.

Approach: three new `IntAlignRule`s in `AlignArgumentsProvider` matching whitespace gaps inside
`IArrayInitializer` entries nested in an object creation's collection initializer. Two settings
in `AlignatorSettingsKey` (reuse `AlignPosition`), two UI items in `AlignatorSettingsSchema`,
NUnit formatter tests with `.cs`/`.cs.gold` data. No changes to existing rules.

**TDD:** settings first (compile only), then the full planned test set goes in and runs RED
(guard tests pass trivially, behavior tests fail because no rule exists yet), then rules are
implemented one at a time until the tests are GREEN. Golds encode the alignment spec (shared
column = max over entries), derived mechanically — never copied from formatter output.

## Planned test cases

All in `CSharpQuirkyFormattingTest.cs`, data files in
`test/data/CodeFormatter/CSharp/AlignArguments/` (2-space indent, method-local
`Dictionary<string, string?> indominusRexFacts = new() { ... }` with the six issue entries:
Name, Diet, Period, Weight, Enclosure, Genome).

| # | Test method | `[TestSetting]`s | Data files | Expected |
|---|-------------|------------------|------------|----------|
| 1 | `TestAlignKeyValuesInCollectionInitializer_SpaceAfterComma` | `INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER = SpaceAfterComma` | `AlignKeyValuesInCollectionInitializer_SpaceAfterComma.cs/.gold` | Values share one column after the longest `{"Key",` prefix |
| 2 | `TestAlignKeyValuesInCollectionInitializer_SpaceBeforeComma` | `... = SpaceBeforeComma` | `...SpaceBeforeComma.cs/.gold` | Commas share one column after the longest `{"Key"` part |
| 3 | `TestAlignClosingBracesInCollectionInitializer` | `INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER = true` | `AlignClosingBracesInCollectionInitializer.cs/.gold` | Entry `}` share one column; inner gaps untouched (key values `DoNotChange`) |
| 4 | `TestAlignKeyValuesInCollectionInitializer_SpaceAfterCommaAndClosingBraces` | both: `SpaceAfterComma` + `true` | `...SpaceAfterCommaAndClosingBraces.cs/.gold` | Both columns; brace column computed over key-padded content |
| 5 | `TestAlignKeyValuesInCollectionInitializer_SpaceBeforeCommaAndClosingBraces` | both: `SpaceBeforeComma` + `true` | `...SpaceBeforeCommaAndClosingBraces.cs/.gold` | Both columns |
| 6 | `TestAlignKeyValuesInCollectionInitializer_DoNotChange` | none (defaults) | `...DoNotChange.cs/.gold` | Output byte-identical to input (default = no-op) |
| 7 | `TestAlignKeyValuesInCollectionInitializer_InlineNoChange` | both: `SpaceAfterComma` + `true` | `...InlineNoChange.cs/.gold` | Single-line initializer unchanged (own-line guard) |

Input block (tests 1–6, identical `.cs` files):

```cs
namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER"/>
internal class <ClassName>
{
  static void IndominusRex()
  {
    Dictionary<string, string?> indominusRexFacts = new()
    {
      {"Name", "Indominus Rex"},
      {"Diet", "Carnivore"},
      {"Period", "Cretaceous"},
      {"Weight", "8,000 kg"},
      {"Enclosure", "Paddock 11"},
      {"Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},
    };
  }
}
```

Gold entry blocks (exact, mechanically derived; `<ClassName>`/`seealso` lines vary per test —
closing-brace tests also reference `INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER`):

- Test 1 (`after`): `{"Name",      "Indominus Rex"},` … `{"Enclosure", "Paddock 11"},` …
  `{"Genome",    "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},` (values at one column)
- Test 2 (`before`): `{"Name"     , "Indominus Rex"},` … `{"Enclosure", "Paddock 11"},` …
  `{"Genome"   , "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},` (commas at one column)
- Test 3 (`close`): `{"Name", "Indominus Rex"                               },` …
  `{"Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},` (braces at one column, inner gaps 1 space)
- Tests 4–5: combination blocks (value/comma column plus brace column over padded content,
  matching the issue's combination examples)
- Test 6: gold == input
- Test 7: single-line `new() { {"Name", "Indominus Rex"}, {"Diet", "Carnivore"}, {"Period", "Cretaceous"} };`
  gold == input

Full exact blocks are in the orchestrator scratchpad (`Alignator-3/gold-blocks.md`) and are
pasted verbatim into the gold files.

**Known issue discrepancy (surfaced, Rule 7):** the issue's pure-mode blocks leave the longest
line (`Genome`) unpadded, contradicting the issue's own combination blocks, which pad it.
Golds pad every entry — consistent with the combination blocks and the feature statement
"so the values of one initializer share a column".

## Step Plan

| Status | Step                                                                                                          | Verification                                                                       |
|--------|---------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------|
| [x]    | 1. Settings: add `INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER` (`AlignPosition`, default `DoNotChange`, alias `alignator_align_key_values_in_collection_initializer`) and `INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER` (`bool`, default `false`, alias `alignator_align_closing_braces_in_collection_initializer`) to `AlignatorSettingsKey.cs` | Test project compiles |
| [x]    | 2. Tests first: all 7 test cases — data files (`.cs` + spec-derived `.gold`) and test methods in `CSharpQuirkyFormattingTest.cs` | `dotnet test --filter "...CSharpQuirkyFormattingTest"`: tests 1–5 RED (no alignment yet), 6–7 pass trivially |
| [x]    | 3. PSI tree dump: temporary debug output from a formatter run over test 1's input; delete the temp code afterwards | Dump read: entry node type, parent chain, children order, gap nodes confirmed; `reference.md` updated |
| [x]    | 4. Key-value rules: `..._SpaceBeforeComma` / `..._SpaceAfterComma` in `AlignArgumentsProvider.cs` (own-line guard + entry-single-line guard, key `$"CollInit$<CommaPart>$Coll{initializerOffset}"`), registered in `Initialize()` | Tests 1–2 GREEN, tests 6–7 still GREEN |
| [x]    | 5. Closing-brace rule: `..._BeforeClosingBrace` (Left = expression, Right = `RBRACE`, same guards, key `$"CollInitClose$BeforeCloseBrace$Coll{initializerOffset}"`) | Tests 3–5 GREEN, all previous still GREEN |
| [x]    | 6. Settings UI: two `ItemFor` entries in `AlignatorSettingsSchema.DescribePart` (dinosaur-dictionary previews) | `./gradlew buildPlugin` succeeds |
| [x]    | 7. Docs + full gate: update AGENTS.md (settings, rules, editorconfig, key files); run `./gradlew testDotNet` and `skirnir-lint` | Full suite green, lint clean, AGENTS.md current |

## Step details

### 1. Settings — `AlignatorSettingsKey.cs`

Follow the existing pattern exactly:

```csharp
[EditorConfigEntryAlias("alignator_align_key_values_in_collection_initializer", EditorConfigAliasType.LanguageSpecificStandard)]
[SettingsEntry(AlignPosition.DoNotChange, "Align key-values in collection initializer")]
public AlignPosition INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER;

[EditorConfigEntryAlias("alignator_align_closing_braces_in_collection_initializer", EditorConfigAliasType.LanguageSpecificStandard)]
[SettingsEntry(false, "Align closing braces in collection initializer")]
public bool INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER;
```

The class-level `[EditorConfigKey("csharp")]` supplies the `csharp_` prefix; the full keys then
match the issue's table. No changes to `AlignPosition.cs`.

### 2. Tests first (RED)

- Data files per the table above; input `.cs` identical for tests 1–6 (class name and
  `seealso` lines vary), golds from the spec blocks.
- Test methods with `[TestSetting]` attributes, named `Test<file base name>`, `DoNamedTest2()`.
- Red check: the formatter output for tests 1–5 equals the input (no rules exist), so the
  gold comparison must fail. Tests 6–7 (gold == input) pass already — they are regression
  guards for the default-off and inline-guard contracts.

### 3. PSI tree dump

Run test 1's input through the formatter with a temporary dump (debug output registered as a
rule that walks the object creation's initializer subtree and prints node types/text).
Confirm: the initializer block node type under `IObjectCreationExpression`; the dictionary
entry node type (expected `IArrayInitializer`); entry children order (LBRACE, key expression,
COMMA, value expression, RBRACE); where whitespace gap nodes sit. Delete the temporary code,
write the confirmed structure into `reference.md`, adjust predicates in steps 4–5 if needed.

### 4. Key-value rules — `AlignArgumentsProvider.cs`

PSI facts (verified, `reference.md` §PSI structure + `psi-dump.txt`): dictionary entries are
`ICollectionElementInitializer` nodes with `ICSharpArgument` children; the initializer block is
`ICollectionInitializer` directly under `IObjectCreationExpression`.

Two rules built with `DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()`, named
`INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER_SpaceBeforeComma` / `..._SpaceAfterComma`,
both `.SwitchOnExternalKey(x => x.INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER)` with
`When(AlignPosition.SpaceBeforeComma)` / `When(AlignPosition.SpaceAfterComma)`
(so `DoNotChange` disables both).

- `Where`: Left = `ICSharpArgument` (key) vs `COMMA` for SpaceBefore; `COMMA` vs
  `ICSharpArgument` (value) for SpaceAfter; `Parent()` = `ICollectionElementInitializer`.
- `Calculate`: verify the entry is dictionary-style (≥2 argument children) and its chain is
  `ICollectionInitializer` → `IObjectCreationExpression`; guards — entry starts its own line
  (walk back via `PreviousNode`: whitespace-with-newline → true, enclosing initializer
  `LBRACE` → true, comma → false); entry subtree contains no newline (walk children checking
  `IWhitespaceNode` text for `'\n'`, mirroring the existing helper). Key:
  `$"CollInit$ArgComma$Coll{collectionInitializerOffset}"` /
  `$"CollInit$ArgAfterComma$Coll{collectionInitializerOffset}"`. No element index needed: the
  engine aligns the k-th same-key gap per line (see `reference.md`, engine behavior).
- Register both in `Initialize()`. XML doc comments in the existing style (summary +
  `<example>` with dinosaur data).
- Reuse the existing static helpers (`PreviousNode`, `NextNode`); add the two small guards next
  to the existing ones rather than duplicating.

### 5. Closing-brace rule — `AlignArgumentsProvider.cs`

`INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER_BeforeClosingBrace`, modeled on
`INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE_SpaceBeforeClosingParen`:

- `Where`: Left = `ICSharpArgument` (the value, last argument of the entry), Right = `RBRACE`,
  `Parent()` = `ICollectionElementInitializer` with ≥2 argument children (dictionary-style
  entries only — simple `{expr}` entries must not be padded).
- `.SwitchOnExternalKey(x => x.INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER)`,
  `When(true).Calculate(...)` with the same guards; key
  `$"CollInitClose$BeforeCloseBrace$Coll{collectionInitializerOffset}"` (single shared column).

### 6. Settings UI — `AlignatorSettingsSchema.cs`

Two `builder.ItemFor(...)` entries appended to `DescribePart`, previews showing a
dinosaur-facts dictionary initializer (`{"T-Rex", "Carnivore"}, {"Velociraptor", "Pack hunter"}, ...`),
`parseType: PreviewParseType.Member`, `previewFixup: IntAlignFixup`, same as every existing item.

### 7. Docs and full gate

- Update `AGENTS.md` (orchestrator): the settings table, alignment-rules table, `.editorconfig`
  example and test counts are brought to full accuracy — including the previously shipped but
  undocumented attribute / argument-names settings, so the tables stay coherent rather than
  4-of-7 complete. Scope creep is limited to documentation accuracy of shipped features.
- `./gradlew testDotNet` — all existing + new tests green.
- `skirnir-lint` — no analyzer findings on changed projects.

## Verification commands

```bash
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj --filter "FullyQualifiedName~CSharpQuirkyFormattingTest"
./gradlew testDotNet
./gradlew buildPlugin
skirnir-lint
```

## Success criteria

All seven planned tests green with spec-derived golds; defaults and inline initializers
untouched; full suite green; lint clean; AGENTS.md current.
