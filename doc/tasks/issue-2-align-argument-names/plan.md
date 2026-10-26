# Plan — Issue #2: Align Argument Names

## Approach

Add a new Alignator alignment rule that pads the whitespace **between a named argument's
`name:` colon and its value** so that, within one argument list, all named-argument values
start in the same column. The rule follows the hand-written `IntAlignRule` pattern of
`INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE_SpaceBeforeClosingParen` (custom `Where` clauses +
custom alignment key), scoped to a single argument list by its start offset — unlike the
existing comma rules, it aligns *within* one call, not *across* statements.

The feature is gated by a new boolean setting `INT_ALIGN_ARGUMENT_NAMES`
(`csharp_alignator_align_argument_names`), default `false`, uniform for invocations,
object creations and attributes. Test data is Jurassic Park themed (Rule 13). README is
updated last, per the issue.

Steps in `doc/tasks/issue-2-align-argument-names/reference.md` (read it first).

## Step Plan

| Status | Step                                                                              | Verification                                                                     |
|--------|-----------------------------------------------------------------------------------|----------------------------------------------------------------------------------|
| [ ]    | 1. Add `INT_ALIGN_ARGUMENT_NAMES` bool setting + editorconfig alias               | `./gradlew compileDotNet` succeeds; existing tests unaffected                    |
| [ ]    | 2. Implement the name-alignment `IntAlignRule` in `AlignArgumentsProvider`        | Compiles; rule registers in `Initialize()` without touching existing rules      |
| [ ]    | 3. Add Jurassic Park themed test data `.cs`/`.cs.gold` + test method              | New test passes; full `CSharpQuirkyFormattingTest` run passes                    |
| [ ]    | 4. Add settings-page entry with dinosaur preview in `AlignatorSettingsSchema`    | Compiles; `skirnir-lint` clean if available                                       |
| [ ]    | 5. Update README (+ CHANGELOG entry) — last step                                  | README table/editorconfig/example match the shipped setting                        |

## Step details

### Step 1 — Setting entry (AlignatorSettingsKey.cs)

* Add to `AlignatorSettingsKey`:
  * `[EditorConfigEntryAlias("alignator_align_argument_names", EditorConfigAliasType.LanguageSpecificStandard)]`
  * `[SettingsEntry(false, "Align argument names")]`
  * `public bool INT_ALIGN_ARGUMENT_NAMES;`
* Model: copy the `INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE` entry (bool, default off).

### Step 2 — Alignment rule (AlignArgumentsProvider.cs)

* New private method `INT_ALIGN_ARGUMENT_NAMES_AfterNameColon()`, called from
  `Initialize()`, modeled on `INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE_SpaceBeforeClosingParen()`:
  * `Where`:
    * `Left().Satisfies(node is the COLON token of a named argument)` — find the colon via
      the parent `ICSharpArgument`'s child tokens (`CSharpTokenType.COLON`); the parent
      must have a non-null argument name.
    * `Right().Satisfies(node is ICSharpExpression)` (the argument value).
    * `Parent().Satisfies(node is ICSharpArgument with name)`, where the argument's
      argument list is an `IArgumentList` (invocation / object creation) or `IAttribute`.
  * `.SwitchOnExternalKey(x => x.INT_ALIGN_ARGUMENT_NAMES, When(true).Calculate(...))`.
  * `Calculate` returns `IntAlignOptionValue($"ArgName${argumentListStartOffset}", AlignatorPriority)`
    — the argument list's document start offset groups exactly the named arguments of
    that one call. Only return a value when the whitespace between colon and value
    contains a line break (see reference open question 2).
* No changes to `BuildIntAlignArgumentsRule` or the existing variants (surgical change).
* Verify compile; register the exact PSI shape findings in this plan if they differ from
  the assumption (reference open question 1).

### Step 3 — Test data + test (Jurassic Park themed)

* New pair `AlignArgumentNames.cs` / `AlignArgumentNames.cs.gold` in
  `test/data/CodeFormatter/CSharp/AlignArguments/` — 2-space indent, CRLF,
  `TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments` namespace, with a
  `<seealso cref="AlignatorSettingsKey.INT_ALIGN_ARGUMENT_NAMES"/>` doc line. Cover:
  1. Multi-line invocation with named args (`Caller.F1(o1: "T-Rex", o2: "Raptor", ...)` —
     one argument per line, values aligned in `.gold`).
  2. Multi-line object creation with named args (`new Velociraptor(o1: ..., ...)`).
  3. Multi-line attribute with named args (`[TestCase(expected: ..., actual: ...)]`).
  4. A mixed named/positional multi-line call (positional args must stay untouched).
  5. A single-line call with named args (must not get padded — pins open question 2).
* Extend `TestData.cs` only if a needed stub shape is missing (e.g. attribute-test fixture
  type or method with parameter names worth aligning).
* New test in `CSharpQuirkyFormattingTest`:
  `[Test] [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENT_NAMES), true)] public void TestAlignArgumentNames() => DoNamedTest2();`
* Verification: `dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj --filter "FullyQualifiedName~CSharpQuirkyFormattingTest"` — all tests green. If the `.gold`
  output differs from expectation, fix the rule or the gold consciously — never edit gold
  just to make the test pass without understanding why (Rule 9).

### Step 4 — Settings page (AlignatorSettingsSchema.cs)

* `.ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENT_NAMES, <dinosaur preview>, parseType: PreviewParseType.Member, previewFixup: IntAlignFixup)`
* Preview: a multi-line named-argument call, e.g. `DinosaurPark` constructor call with
  `o1:`..`o6:` named args on separate lines, so the live formatter demo shows the columns.
* Verification: compile; if feasible, quick `./gradlew runIde` spot-check of the settings
  page (optional — note if skipped).

### Step 5 — README + CHANGELOG (last, per the issue)

* `README.md`:
  * Features list: new bullet "Align argument names in multi-line calls".
  * Settings table row: `Align argument names` | aligns `name:` values in one column | `true`, `false` | `csharp_alignator_align_argument_names`.
  * `.editorconfig` sample: `csharp_alignator_align_argument_names = true`.
  * New before/after example using the themed test data.
* `CHANGELOG.md`: entry under a new version heading (Keep-a-Changelog).
* Verification: manual review — every documented alias/name matches `AlignatorSettingsKey`
  exactly; no stale claims.

## Verification per step (summary)

* Steps 1, 2, 4: `./gradlew compileDotNet` (and `skirnir-lint` when available).
* Step 3: filtered `dotnet test` run, then full test project run.
* Step 5: README/CHANGELOG consistency check against the shipped code.
* Final: `./gradlew testDotNet` for the whole suite; confirm no file outside scope changed
  (`git status` review).
