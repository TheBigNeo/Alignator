# Plan — Alignator #13 Cleanup

Cleanup in four phases: secure the baseline, fix defects, remove redundancy, close comment and
doc gaps. Every step is behavior-preserving unless it explicitly fixes a defect, and every step
is verified by the full test suite (43 tests; 44 from step 2 on). No commits (AGENTS.md); CHANGELOG.md untouched.

## Approach

The 24 alignment rules are verified exclusively by gold-file formatter tests, so the existing
suite is a reliable safety net for refactoring: any rule consolidation that changes behavior
fails loudly. The strategy is therefore:

1. Run the baseline and record it.
2. Fix the two real defect candidates first (A1, A2/A3) — small, isolated, each with its own test.
3. Then remove duplication bottom-up (base first, then the providers that consume it), one
   extraction per step, always keeping the rule names and alignment keys byte-identical so the
   gold files cannot change.
4. Add the missing comments last, when the code has its final shape, so no comment documents
   an intermediate state.
5. Sync docs and package metadata; resolve the open questions with experiments, not guesses.

Invariant across all steps: rule count (24), rule names, alignment keys, settings keys, and
`.editorconfig` aliases stay unchanged. Nothing in this plan requires the "no public version"
freedom — it is not used.

## Step Plan

| Status | Step | Verification |
|--------|------|---------------|
| [x] | 1. Baseline: run full suite, record green count | `dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj` — 43/43 green; if not green, stop and report |
| [x] | 2. Defect A1: add `ctx.GetLinebreaksNumber() > 0` guard to `AttributeArgumentsProvider` closing-paren rule; add test `TestAlignClosingParenInAttribute_WrappedParenOwnLineUntouched` + `.cs`/`.cs.gold` pair with multi-line attribute whose `)` opens its own line | New test green; full suite green; if gold shows the guard changes nothing, keep guard + document why in a code comment |
| [x] | 3. Defect A2/A3: normalize line endings in `FunctionArgumentsFormattingTest.cs` (10 LF lines), fix `\r\r\n` line 1 of `AllSettingsAtOnceCoverageTest.cs`, delete dead commented-out `ZoneMarker` line in `TestEnvironment.cs` | `git diff` shows only whitespace/whitespace-only and one deleted comment line; build + full suite green |
| [x] | 4. Redundancy B1: in `AlignArgumentsProviderBase`, extract shared helpers `TryGetArguments(parent)`, `UsesNamedArguments(arguments)`, `FindAlignmentBlockOffset(node, variant)`; both builders call them; add `using System.Collections.Generic;`/`using System.Linq.Expressions;` and drop fully-qualified `System.*` types (B6) | Full suite green; gold files unchanged (`git status` clean on `test/data`); rule count still 24 (rule names untouched) |
| [x] | 5. Redundancy B5: unify the three line-start scanners into one parameterized helper in the base (`StartsOwnLineAfterToken(node, openTokenType)`), used by `EntryStartsOwnLine` call sites and by `NamedArgumentsProvider.StartsLineOrOpensList`; `StartsOwnLine` becomes the no-open-token variant | Full suite green; gold files unchanged |
| [x] | 6. Redundancy B3: in `CollectionInitializerArgumentsProvider`, extract `TryGetAlignableInitializer(entry)` (null + argument-count + parent-initializer + own-line + linebreak guards); both rules call it | Full suite green; gold files unchanged |
| [x] | 7. Redundancy B4: extract shared `BuildIntAlignClosingParenRule(variant)` into the base; `FunctionArgumentsProvider` and `AttributeArgumentsProvider` reduce to thin variant definitions; rule names and keys stay byte-identical | Full suite green; gold files unchanged; rule count still 24 |
| [x] | 8. Redundancy B7 (moderate): merge each SpaceBefore/SpaceAfter method pair in Constructor/Function/Attribute providers into one method per setting that loops over the two positions; method names keep the settings-key prefix (e.g. `ALIGN_CONSTRUCTOR_ARGUMENTS_CommaPositions`); doc comments stay per rule | Full suite green; rule count 24; Initialize() call list readable |
| [x] | 9. Redundancy B2: replace `GetText() == "="` with `CSharpTokenType.EQ` token checks in `NamedArgumentsProvider` (lines 64, 318) | Named + Mixed tests green; full suite green |
| [x] | 10. Comments C: add doc comment to `AlignPosition`; to `AlignatorSettingsSchema` + `IntAlignFixup` (what the fixup disables and why); to `TestEnvironment.SetJetTestPackagesDir` (JET_TEST_PACKAGES_DIR fallback logic); fix `NextSolidSibling` doc to describe the self-fallback (A4); one-liner for `IAlignatorZone` | Build green; comments describe behavior only, no boilerplate |
| [x] | 11. Question 1 (engine gate): experiment — remove/flip `INT_ALIGN_INVOCATIONS` vs `INT_ALIGN_VARIABLES` in one function test locally; record which switch actually gates Alignator rules; fix README wording if it overstates (`int_align_variables` vs context-dependent switches); revert experiment | Experiment result documented in reference.md; README/DEVELOPERS corrected accordingly |
| [x] | 12. Cosmetics D: schema FUNCTION_* preview indentation (8→4 spaces), `ZoneMarker.cs`/`NamedArgumentsProvider` double blank lines, misaligned doc example in `ConstructorArgumentsProvider` | Build green; previews render unchanged (raw-string leading whitespace in UI previews is cosmetic — check settings page visually via `runIde` or leave as-is if risk) |
| [x] | 13. Docs A6: update AGENTS.md (24 rules, 13 settings, per-context test counts, 43 tests incl. guard), check DEVELOPERS.md rule/settings tables against code | Table counts match `grep -c` numbers from this reference |
| [x] | 14. Metadata A5: replace `Description`/`Authors` placeholders in `Plugin.props`, `<p>Sample description</p>` in `plugin.xml`, remove `<!-- TODO -->` in `Alignator.csproj`. Needs maintainer input for real texts — ask before filling | `./gradlew buildPlugin` succeeds; nupkg metadata shows the agreed texts |
| [x] | 15. Final regression: full suite; update Step Plan statuses; report | 44/44 green, build 0 warnings, `git diff` reviewed against scope |

## Affected files per step

1. — (no files)
2. `AttributeArgumentsProvider.cs`; `AttributeArgumentsFormattingTest.cs`; `test/data/.../Attributes/AlignClosingParenInAttribute_WrappedParenUntouched.cs` + `.gold`
3. `FunctionArgumentsFormattingTest.cs`, `AllSettingsAtOnceCoverageTest.cs`, `TestEnvironment.cs`
4. `AlignArgumentsProviderBase.cs`
5. `AlignArgumentsProviderBase.cs`, `NamedArgumentsProvider.cs` (and its callers of `EntryStartsOwnLine` unchanged)
6. `CollectionInitializerArgumentsProvider.cs`
7. `AlignArgumentsProviderBase.cs`, `FunctionArgumentsProvider.cs`, `AttributeArgumentsProvider.cs`
8. `ConstructorArgumentsProvider.cs`, `FunctionArgumentsProvider.cs`, `AttributeArgumentsProvider.cs`
9. `NamedArgumentsProvider.cs`
10. `AlignPosition.cs`, `AlignatorSettingsSchema.cs`, `TestEnvironment.cs`, `NamedArgumentsProvider.cs`, `IAlignatorZone.cs`
11. `README.md`, `DEVELOPERS.md` (wording only)
12. `AlignatorSettingsSchema.cs`, `ZoneMarker.cs`, `NamedArgumentsProvider.cs`, `ConstructorArgumentsProvider.cs`
13. `AGENTS.md`, `DEVELOPERS.md`
14. `Plugin.props`, `plugin.xml`, `Alignator.csproj`
15. — (verification only)

## Notes

- Steps 2–3 fix defects; steps 4–9 are pure refactors verified by the gold files; steps 10–14
  are documentation and metadata. Steps within a phase are independent of each other except that
  4 should precede 5–7 (helpers first) and 11 should precede 13 (engine-gate wording feeds the
  doc sync).
- Step 8 is the only step that changes method *names* inside providers; rule names, alignment
  keys and settings keys are untouched, so gold files and user-facing behavior cannot change.
  If preferred, step 8 can be dropped — steps 4–7 already remove the bulk of the duplication.
- Step 12's preview-indentation change touches raw strings the settings page renders; if any
  doubt remains after checking, skip that single cosmetic change rather than risk the preview.
- Execution follows Phase 4 of PlanRules.md: one step at a time, subagent with fresh context per
  step, orchestrator verifies by reading actual test output.
