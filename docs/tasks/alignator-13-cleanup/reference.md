# Reference — Alignator #13 Cleanup (Full Code Review & Clean-Up Plan)

## Goal and acceptance criteria

**Goal:** Project-wide code review of every class and function; then a step plan that removes
redundant code, closes comment gaps (no boilerplate), fixes real defects, and syncs the docs —
so a clean project remains at the end.

**Acceptance criteria:**
- All 43 tests green after every step (`dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj`).
- No behavior change in any refactoring step (gold files untouched unless a step explicitly fixes a defect with a new test).
- All 24 alignment rules still registered after consolidation steps.
- Docs (AGENTS.md, DEVELOPERS.md, README.md) match the actual code (counts, settings).

## Relevant files and key symbols

| File | Key symbols | Review result |
|------|-------------|---------------|
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProviderBase.cs` | `BuildIntAlignArgumentsRule`, `BuildIntAlignFirstArgumentRule`, `EntryStartsOwnLine`, `ContainsLineBreak`, `PreviousNode`, `NextNode`, `ArgumentAlignVariant` (record) | Both rule builders duplicate ~60% of their logic; fully-qualified `System.*` types without usings; `ArgumentAlignVariant` well designed |
| `.../ConstructorArgumentsProvider.cs` | 8 rule methods, `IsFieldOrPropertyAssignmentStatement` | 8 methods identical except one enum value / predicate / key group |
| `.../FunctionArgumentsProvider.cs` | 4 + closing-paren rule | Closing-paren rule has `ctx.GetLinebreaksNumber() > 0` guard |
| `.../AttributeArgumentsProvider.cs` | 4 + closing-paren rule, `AttributeSectionListOffset` | Closing-paren rule **lacks** the linebreak guard (see Findings B1); no empty-args check needed (Where clause already excludes it) |
| `.../NamedArgumentsProvider.cs` | `ALIGN_ARGUMENTS_NAMED_AfterNameColon`, `ALIGN_ARGUMENTS_NAMED_ALL_ArgumentIndent`, `StartsOwnLine`, `StartsLineOrOpensList`, `GapAfterNameTokenContainsLineBreak`, `FindNameToken`, `IsInsideAttributeArgument`, `NextSolidSibling` | Three private backward/forward whitespace scanners overlap with base `EntryStartsOwnLine`; `GetText() == "="` string compares (lines 64, 318) instead of `CSharpTokenType.EQ`; `NextSolidSibling` doc does not mention the self-fallback |
| `.../CollectionInitializerArgumentsProvider.cs` | `ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES_BeforeClosingBrace`, `BuildIntAlignKeyValuesInCollectionInitializerRule` | Both rules duplicate the 5-guard entry validation block (~30 lines) |
| `.../WithExpressionAssignmentsProvider.cs` | `ALIGN_WITH_EXPRESSION_ASSIGNMENTS_BeforeEq` | Clean; manual ancestor walk could be a helper but is documented |
| `.../AlignatorSettingsSchema.cs` | `DescribePart`, `IntAlignFixup` | Class and `IntAlignFixup` lack doc comments; FUNCTION_* preview raw strings indented 8 spaces (others 4) |
| `.../AlignatorSettingsKey.cs` | 13 settings fields, regions | Exemplary documentation; nothing to change |
| `.../AlignPosition.cs` | enum | No doc comment (`FirstArgumentPosition` has one) |
| `src/dotnet/Alignator/Helpers/NodeHelpers.cs` | `GetContainingBlockOffset` | Two sequential ancestor walks could be one; doc is good |
| `src/dotnet/Alignator/IAlignatorZone.cs`, `ZoneMarker.cs` | zone wiring | `ZoneMarker.cs`: double blank line; no docs (acceptable for boilerplate zone classes) |
| `src/dotnet/Alignator.Tests/TestEnvironment.cs` | `AlignatorTestsAssembly`, `SetJetTestPackagesDir` | Dead commented-out line 28; no doc comments |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/*.cs` | 8 test classes, 43 tests | Consistent pattern; `FunctionArgumentsFormattingTest.cs` has 10 LF-only lines among CRLF (mixed endings); `AllSettingsAtOnceCoverageTest.cs` line 1 ends `\r\r\n` (stray CR) |
| `src/dotnet/Alignator.Tests/Data/*` | stubs, `TestAlignatorAttribute` | Fine; `TestCaseAttribute` has case-sensitivity comment |
| `src/dotnet/Plugin.props` | package metadata | Placeholders: `Description`, `Authors`, empty URLs |
| `src/rider/main/resources/META-INF/plugin.xml` | descriptor | `<p>Sample description</p>` placeholder |
| `src/dotnet/Alignator/Alignator.csproj` | — | Template leftover `<!-- TODO: add additional assemblies -->` |

## Current behavior and why the change is needed

The codebase is in good shape overall: consistent naming convention (rule methods named after
their settings), meaningful non-boilerplate comments, bidirectional `<seealso>` links between
tests and test data, a coverage guard test, and a documented settings-key scheme. The cleanup
targets are duplication between the rule builders/providers, a small set of hygiene defects,
a possibly missing engine guard in the attribute closing-paren rule, and stale counts in the
docs. There is **no public release yet** — internal renames and key changes are still cheap,
but no key rename is required by this plan.

## Constraints, conventions and gotchas

- AGENTS.md: DO NOT COMMIT; DO NOT TOUCH CHANGELOG.md; Rule 13 Jurassic Park names for example data.
- Rule names (`Name(...)`) and alignment-key strings (`Params$...`, `Attr$...`) are internal but must stay unique per rule; consolidation must not change them without need.
- One-method-per-setting-name convention in providers is documented in AGENTS.md "Extending the Plugin" — keep names aligned to settings keys even when merging method bodies.
- Test data `.cs` files are compiled AND read as text; whitespace-only edits to test files are safe, edits to data files change test semantics.
- Tests run via `dotnet test` (one project) or `./gradlew testDotNet`.
- Test framework: gold-file comparison (`DoNamedTest2`), so refactors are verified purely by the existing suite — gold files are the safety net.

## Open questions / risky assumptions

1. **INT_ALIGN_VARIABLES vs INT_ALIGN_INVOCATIONS:** README claims every Alignator alignment
   requires built-in `int_align_variables = true`, but the formatter tests set
   `INT_ALIGN_INVOCATIONS = true` (only `TestAllSettingsAtOnce` sets `INT_ALIGN_VARIABLES`).
   Either the README overstates the gate or the tests rely on defaults that differ from what
   users have. Needs one experiment: toggle `INT_ALIGN_VARIABLES` in one test and observe.
   Not resolved by guessing (Rule 1).
2. **Attribute closing-paren linebreak guard (Finding B1):** assumption is that the engine
   never pads gaps containing a line break, making the guard defensive-only — but the function
   rule's comment claims the guard is required. The new test in Step 2 decides; if the guard
   changes nothing, keep it anyway for symmetry and document why.
3. **Package metadata placeholders:** real description/author text is a content decision of
   the maintainer — plan asks, does not guess.

## Review findings (grouped)

### A. Defects / inconsistencies
- **A1** `AttributeArgumentsProvider` closing-paren rule lacks the `ctx.GetLinebreaksNumber() > 0`
  guard that `FunctionArgumentsProvider` has; README promises wrapped `)` is never padded.
- **A2** `FunctionArgumentsFormattingTest.cs`: 10 LF-only lines among CRLF (mixed endings);
  `AllSettingsAtOnceCoverageTest.cs`: line 1 ends `\r\r\n`.
- **A3** `TestEnvironment.cs`: commented-out dead `ZoneMarker` line.
- **A4** `NextSolidSibling` doc comment does not match behavior (returns the node itself when
  no solid sibling exists — deliberate fallback, undocumented).
- **A5** `Plugin.props` / `plugin.xml` / `Alignator.csproj` template placeholders and TODO comment.
- **A6** Docs stale: AGENTS.md "22 alignment rules" (actual 24), "Twelve settings" (actual 13),
  per-context test counts (Constructor 9→10, Function 5→8, Named 1→2, Mixed 4→6), "35 formatter
  tests" (actual 42 + 1 guard).

### B. Redundancy (clean code)
- **B1** Base: the two rule builders share the arguments-fetch, named-args stand-back,
  statement walk-up, block-offset and key-building logic; also switch-statement vs
  switch-expression inconsistency for the same "get arguments" job.
- **B2** `NamedArgumentsProvider`: `GetText() == "="` instead of `CSharpTokenType.EQ` (2 places).
- **B3** `CollectionInitializerArgumentsProvider`: identical 5-guard entry validation in both rules.
- **B4** Function/Attribute closing-paren rules: near-identical structure (~45 lines each).
- **B5** Three overlapping line-start scanners (`EntryStartsOwnLine` in base,
  `StartsOwnLine` + `StartsLineOrOpensList` in `NamedArgumentsProvider`) — same walk, different stop tokens.
- **B6** Fully-qualified `System.Collections.Generic.IList`, `System.Linq.Expressions.Expression<System.Func<...>>`,
  `System.Func` in the base (no System usings in the file).
- **B7** Providers: Before/After method pairs per rule identical except one enum value
  (Constructor 8, Function 4, Attribute 4 rule methods).

### C. Missing comments (no boilerplate)
- `AlignPosition` enum (contrast: `FirstArgumentPosition` documented).
- `AlignatorSettingsSchema` class + `IntAlignFixup`.
- `TestEnvironment` classes (`SetJetTestPackagesDir` JET_TEST_PACKAGES_DIR dance is non-obvious).

### D. Readability cosmetics (low priority)
- Schema previews: FUNCTION_* raw strings indented 8 spaces, others 4.
- `ZoneMarker.cs` double blank line; double blank lines inside `NamedArgumentsProvider`.
- `ConstructorArgumentsProvider` doc example column alignment off by one in the SpaceBeforeComma
  assignment example.

## Resolved questions (experiments, not guesses)

1. **Engine gate (Question 1):** The gate is `INT_ALIGN_INVOCATIONS`, not `INT_ALIGN_VARIABLES`.
   Experiments (constructor/attribute/with/collection/named test classes, one variable flipped
   at a time, reverted afterwards):
   - `INT_ALIGN_INVOCATIONS = false` (class attribute flipped): every alignment test of every
     context fails - only the tests whose gold expects *no* padding still pass. No Alignator
     rule aligns in any context without it.
   - `INT_ALIGN_VARIABLES = false` (added, `INT_ALIGN_INVOCATIONS = true` kept): full suite
     green - `INT_ALIGN_VARIABLES` gates nothing the Alignator gold files rely on.
   - `INT_ALIGN_VARIABLES = true` alone (`INT_ALIGN_INVOCATIONS` left at default): the built-in
     `=` alignment of variable declarations appears, but no argument padding - 6 of 10
     constructor tests fail. With `INT_ALIGN_INVOCATIONS = true` the gold's `=` alignment of
     variable declarations appears too (verified by the passing suite with
     `INT_ALIGN_VARIABLES = false`).
   Consequence applied: README/DEVELOPERS wording and the settings schema's gate entry now use
   `int_align_invocations`; the schema entry binds `CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS`
   with an invocation sample. A probe of the built-in switch's own effect (probe test, reverted)
   showed no visible formatting change on multi-line invocations - for Alignator users the
   switch is effectively only the engine gate.

2. **Attribute closing-paren linebreak guard (Question 2):** defensive-only. With the guard
   disabled (`if (false && ctx.GetLinebreaksNumber() > 0)`) the new wrapped-paren test still
   passes - the engine leaves gaps that contain a line break unpadded by itself. The guard is
   kept for symmetry with the function variant; the code comment documents both facts. The
   function rule's comment claim that the guard is required is therefore also softened in the
   shared builder: the guard makes the intent explicit, the engine is the actual enforcer.

3. **`dependencies.json` build blocker (found during step 14 verification):** `./gradlew
   buildPlugin` failed at `/t:Pack` with NU5019 - the Alignator.csproj packs
   `..\..\..\dependencies.json` and build.gradle.kts copies it into META-INF, but the file
   is a gitignored build artifact that nothing in the SDK 2025.3 toolchain regenerates (no
   generator exists in the installed JetBrains.ReSharper.SDK / Rider.SDK packages), and the
   previously built `output/Alignator-9999.0.0.zip` did not contain it either - the references
   pointed at a file that has not existed in any artifact this toolchain ever produced.
   Resolution: the dangling references were removed from Alignator.csproj and
   build.gradle.kts; `./gradlew buildPlugin` now succeeds and produces zip + nupkg.
   Maintainer note: if an older ReSharper SDK workflow required the file, that requirement is
   gone with SDK 2025.3; if a future SDK reintroduces it, restore the references and a generator.
