# Alignator-3 — Dictionary Collection Initializer: Reference

## Goal

Align `{"Key", Value}` dictionary-style entries inside collection initializers by padding
whitespace, so all entries of one initializer share columns:

- Key-value alignment (`DoNotChange` / `SpaceBeforeComma` / `SpaceAfterComma`): keys (or values)
  of all entries share one column.
- Closing-brace alignment (bool, default `false`): the `}` of every entry shares one column;
  combines with every key-value mode, including `DoNotChange`.

## Acceptance criteria

1. The five outputs in the issue ("Expected" section) are reproduced by automated formatter tests.
2. Defaults (`DoNotChange`, `false`) leave code byte-identical (no Alignator effect).
3. Settings configurable via .editorconfig:
   - `csharp_alignator_align_key_values_in_collection_initializer` = `do_not_change` / `space_before_comma` / `space_after_comma`
   - `csharp_alignator_align_closing_braces_in_collection_initializer` = `true` / `false`
4. Both settings appear in Settings → Code Editing → C# → Code Style → Others → Alignator page,
   with Jurassic Park dinosaur preview code (Rule 13).
5. Existing rules and tests are unaffected.

## Relevant files and symbols

| File | Symbols |
|------|---------|
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProvider.cs` | `AlignArgumentsProvider.Initialize()`; rule builders `INT_ALIGN_ARGUMENTS_IN_*`, `INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE_SpaceBeforeClosingParen` (pattern for closing-token rule), `INT_ALIGN_ARGUMENT_NAMES_AfterNameColon` (pattern for line-break guards); helpers `StartsLineOrOpensList`, `GapAfterNameTokenContainsLineBreak`, `PreviousNode`, `NextNode`; shared builder `BuildIntAlignArgumentsRule` + `ArgumentAlignVariant` (argument-list-shaped only — collection initializers need their own rules) |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs` | `AlignatorSettingsKey` — settings fields with `[EditorConfigEntryAlias]`; class carries `[EditorConfigKey("csharp")]`, which supplies the `csharp_` prefix of every alias |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs` | `DescribePart` — `builder.ItemFor(...)` entries with preview snippets, `parseType: PreviewParseType.Member`, `previewFixup: IntAlignFixup` |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignPosition.cs` | `AlignPosition` enum (`DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`) — reuse, do not extend |
| `src/dotnet/Alignator/Helpers/NodeHelpers.cs` | `GetContainingBlockOffset` (block-scoped alignment groups — not needed here; initializer-scoped offset is local to the entry) |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/CSharpQuirkyFormattingTest.cs` | Test class: `[TestAlignator]`, class-level `[TestSetting]`s (`WRAP_LINES=false`, `INT_ALIGN_INVOCATIONS=true`), per-test `[TestSetting]` + `DoNamedTest2()` |
| `src/dotnet/Alignator.Tests/test/data/CodeFormatter/CSharp/AlignArguments/*` | `.cs` / `.cs.gold` pairs, 2-space indent, `seealso` doc comments referencing the exercised settings |
| `src/dotnet/Alignator.Tests/Data/TestData.cs` | Dummy types used by test data (`StringSetting` etc.) |

## How the formatter integration works (from the existing rules)

- Rules are `IntAlignRule`s registered via `DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()`
  in `Initialize()`, priority `100_000` (`AlignatorPriority`).
- A rule matches a whitespace *gap* through `Where(Left(), Right(), Parent())` and returns an
  `IntAlignOptionValue(key, priority)` from `When(value).Calculate(...)`. All gaps with the same
  key are padded to a common column; gaps returning `null` are untouched.
- `.SwitchOnExternalKey` + `When(variant)` restrict a rule to one setting value, so several rules
  can share one setting (e.g. `SpaceBeforeComma` and `SpaceAfterComma` rules both switch on the
  same `AlignPosition` field; each matches only its own value, `DoNotChange` matches neither).
- Alignment group = unique part of the key (usually a document offset). Arguments-in-block rules
  use `GetContainingBlockOffset()`; per-call rules (argument names) use the argument-list offset.
  For this feature the group is the collection initializer block of one object creation —
  entries of different initializers must never align with each other.
- Comma alignment keys include the argument index so different columns stay independent; the
  closing-paren rule uses a single shared key (one column).

## PSI structure for dictionary-style entries (VERIFIED, see psi-dump.txt)

For `Dictionary<string, string?> d = new() { {"Name", "Indominus Rex"}, ... };` the R# 2025.3
PSI tree is:

```
ObjectCreationExpression (IObjectCreationExpression)
├─ NEW_KEYWORD 'new', LPARENTH, ArgumentList (empty), RPARENTH
├─ NewLine, Whitespace
└─ CollectionInitializer (ICollectionInitializer)     ← initializer block, direct child
   ├─ LBRACE '{'
   ├─ [per entry:] CollectionElementInitializer (ICollectionElementInitializer)
   │    ├─ LBRACE '{'
   │    ├─ CSharpArgument → CSharpLiteralExpression → STRING_LITERAL '"Name"'   (key)
   │    ├─ CommaTokenElement COMMA
   │    ├─ Whitespace ' '                              (after-comma gap; key→comma gap
   │    │                                                has NO whitespace node in tight input)
   │    ├─ CSharpArgument → … '"Indominus Rex"'                                    (value)
   │    └─ RBRACE '}'
   ├─ CommaTokenElement, NewLine, Whitespace           ← separators between entries
   └─ RBRACE '}'
```

Facts that drive the rule predicates:

- Dictionary-style entries are `ICollectionElementInitializer` with `ICSharpArgument` children
  — NOT `IArrayInitializer` as first assumed. `IArrayInitializer` covers array literals.
- Simple `{expr}` entries are also `ICollectionElementInitializer` (one argument, no comma):
  the key-value rules never match them (no inner COMMA); the closing-brace rule must
  explicitly require ≥2 arguments (a comma inside).
- `Parent()` of the key→comma and comma→value gaps is the entry itself
  (`ICollectionElementInitializer`); Left/Right are the `ICSharpArgument` wrappers and the
  COMMA / RBRACE tokens — NOT raw `ICSharpExpression`.
- A gap with no whitespace node at all (tight `"Name",`) still matches and gets padded —
  proven by the shipped attribute SpaceBeforeComma test on the same shape.
- `ICollectionInitializer` appears directly under `IObjectCreationExpression`; its start
  offset is the alignment group. With-expression entries (`{ Height = 2 }`) are a different
  node type and never match.

Token types: `CSharpTokenType.COMMA`, `CSharpTokenType.LBRACE`, `CSharpTokenType.RBRACE`.

## Current behavior and why the change is needed

No existing rule touches collection initializers: the argument rules require
`Parent() = IArgumentList` (of object creation / invocation) or `IAttribute`; the argument-names
rule requires an `ICSharpArgument` with `ArgumentName` or an attribute assignment. Gaps inside
`IArrayInitializer` entries have parent `IArrayInitializer`, so none match. Dictionary entries
therefore stay ragged (the "Actual" block of the issue).

## Decisions and assumptions

1. **Entry predicate**: an `ICollectionElementInitializer` with ≥2 `ICSharpArgument` children
   (dictionary-style `{"K", V}` entry) whose enclosing chain is `ICollectionInitializer` →
   `IObjectCreationExpression`. No dictionary-type resolution: any two-or-more-element entry in
   an object-creation initializer is aligned.
2. **Out of scope**: simple `{expr}` entries, index initializers `{[k] = v}`, array literals,
   `with`-expression initializers. They are skipped, never broken.
3. **Own-line guard** (mirrors `INT_ALIGN_ARGUMENT_NAMES_AfterNameColon`, commit a731fea): a gap is
   aligned only when its entry starts its own line — preceded by a line break or opening the
   initializer. Inline initializers (`new() { {"a","b"}, {"c","d"} }` on one line) stay untouched.
4. **Line-break-in-gap guard** (mirrors `GapAfterNameTokenContainsLineBreak`): when the gap
   itself contains a newline (wrapped key/value), it is skipped.
5. **Key layout**: key-value rules use one shared key per collection initializer (no element
   index — the engine aligns the k-th same-key gap per line; see Findings). Closing-brace rule
   uses a single shared key, like the attribute closing-paren rule.
6. **Settings names follow the issue's editorconfig keys exactly** (note: `braces` plural,
   unlike the existing `INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE`): field names
   `INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER` and
   `INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER`.

## Constraints, conventions, gotchas

- **Gold comparison is byte-exact**: gold files must have NO trailing newline (existing golds
  end with `}`). BOM differences are normalized, trailing newlines are not
  (`TestUtil.CheckDataAgainstGoldFile` with `compareLineEndings`).
- **Base formatter rewrites `{"Key", Value}` to `{ "Key", Value }`**: the R# default
  `CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES = true`. The new
  tests set it to `false` per-test (matching the issue's tight-brace style). It must stay
  per-test — the existing constructor golds contain `{ Height = 2 }` object initializers that
  depend on the default. The setting also tightens the OUTER initializer braces of a
  single-line initializer (`new() {{...}}`), which the inline test's data reflects.
- Test data files: 2-space indent; file-scoped namespace
  `TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments`; `seealso` doc comments
  for exercised settings; one class per test file, named like the test.
- Test data is parsed, not compiled — no stub types or usings needed for
  `Dictionary<string, string?>`.
- `skirnir-lint` gates analyzers on changed projects.
- Doc comments on new rule methods follow the existing style: summary + `<example>` with
  dinosaur-flavored code (Rule 13).
- AGENTS.md documents settings and rules; its tables must be extended with the two new settings
  (the file lists every current rule and setting — previous features updated it).

## Findings (verified against shipped code)

- **Issue #3 typo (conflict surfaced, Rule 7):** the issue's pure-mode blocks leave the longest
  entry (`Genome`) unpadded, contradicting its own combination blocks, which pad it. Golds pad
  every entry — consistent with the combination blocks and "the values share a column".
- **Engine aligns the k-th same-key gap per line.** The existing comma rules compute a constant
  per-rule index (their scan always returns the first argument adjacent to a comma), yet
  `AlignArgumentsInConstructor_SpaceAfterComma.cs.gold` shows per-index columns across lines.
  So the IntAlign engine aligns the k-th same-key gap occurrence of each line as one column
  group. Consequence: dictionary-entry keys need **no element index** — one shared key per
  initializer is enough, and 3+-element entries degrade gracefully instead of collapsing.
- `CSharpTokenType.LBRACE` / `RBRACE` exist (confirmed in the 2025.3 SDK).

## Open questions / risks

- **PSI nesting to confirm in step 1**: whether the dictionary entry's parent is the
  `ICollectionInitializer` directly or an intermediate node, and the exact type of the object
  creation's initializer block. If the tree differs (e.g. entries are not `IArrayInitializer`),
  the predicates in steps 3–4 are adjusted — the rule shape stays the same.
- **Risky assumption**: that the IntAlign engine composes key-value and closing-brace alignment
  on the same lines (two different keys, both padding the same entry). Precedent: the attribute
  comma + closing-paren rules already do this (`AlignArgumentsInAttribute_SpaceBeforeCommaAndClosingParen` test).
- If the own-line guard proves unnecessary (engine already ignores same-line gaps), it can be
  dropped — the tests decide, not the reviewer.
