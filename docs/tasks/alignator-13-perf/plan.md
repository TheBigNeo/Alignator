# Plan — Alignator #13: Formatter Hot-Path Performance

Read `AGENTS.md`, `docs/PlanRules.md` and `reference.md` (same folder) before implementing.
Execute one step at a time, one subagent per step, verify each step yourself (PlanRules Phase 4).

Do not commit. Do not touch CHANGELOG.md. Perf fix #1 was made obsolete and #2 was rejected —
see reference.md; neither appears below.

## Step 1 — Single ancestor walk in `GetContainingBlockOffset` (perf, no behavior change)

File: `src/dotnet/Alignator/Helpers/NodeHelpers.cs`

- Replace the two sequential `while (n != null)` walks with ONE walk that returns at the first
  `IBlock` (checked first) or the first `ICSharpTypeDeclaration`, and returns `null` only when
  the chain ends without either.
- Update the doc comment to describe the single walk.
- Equivalence is verified against the SDK (reference.md: no type declaration can sit below a
  block; attribute rules never call this helper).

Verification: full test run green, no gold file touched:

```bash
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj
```

## Step 2 — Early type check in the named-argument rule (perf, no behavior change)

File: `src/dotnet/Alignator/Psi/CodeStyle/Formatting/NamedArgumentsProvider.cs`

In the `Calculate` callback of `ALIGN_ARGUMENTS_NAMED_AfterNameColon`, directly after the
`gapParent is null` check and BEFORE `FindNameToken`, insert:

```csharp
// Both accepted forms have an argument-shaped gap parent: the 'name:' colon gap sits inside
// an ICSharpArgument, the attribute 'Name = value' assignment gap inside an IPropertyAssignment.
// Every other '=' or ':' gap - ordinary assignments, ternaries, case labels, object
// initializers, with expressions - is rejected here without walking anything.
if (gapParent is not (ICSharpArgument or IPropertyAssignment))
{
    return null;
}
```

- Keep `FindNameToken` (it now runs only on the small argument/property-assignment subtree) and
  keep `IsInsideAttributeArgument` unchanged — the `=` path still needs it, because
  object-initializer `IPropertyAssignment`s pass the type check.
- Add one sentence to the method's doc comment: malformed error-recovery PSI such as
  `[Test(Name = x = 1)]` no longer receives an alignment key — this is a deliberate bug fix.
- The named rule tests (`Named/AlignArgumentNames.cs`) and the Mixed tests cover both accepted
  shapes; no test data changes.

Verification: full test run green, no gold file touched (same command as step 1).

## Step 3 — Attribute named arguments stand back from comma alignment (bug fix, behavior change)

Root cause and engine facts: reference.md ("Current behavior", point 3). `TryGetArguments` /
`UsesNamedArguments` cannot see `IAttribute.PropertyAssignments`.

### 3.1 — Stand-back check in the shared rule builders

File: `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProviderBase.cs`

- First verify the exact member shape of `IAttribute.PropertyAssignments` in the ReSharper SDK
  (collection type / count accessor) — do not guess.
- In `BuildIntAlignArgumentsRule` and in `BuildIntAlignFirstArgumentRule`, extend the existing
  `UsesNamedArguments(arguments)` stand-back so that an `IAttribute` parent with at least one
  property assignment also stands back. Keep it as one cheap check on `ctx.Parent.NodeOrNull`
  (the gap's parent is the attribute itself). Match the existing comment style; state that
  named attribute arguments live in `PropertyAssignments`, not in `Arguments`.
- Do NOT change the attribute closing-paren rule: fully-named attribute lists can never match
  its `Where` (left of the gap is inside an `IPropertyAssignment`), which is consistent with
  stand-back. Add one clarifying sentence to the closing-paren doc comment only if the existing
  text does not already cover it.

### 3.2 — New test: mixed attribute list stands back

Files:

- Test method in `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/MixedContextsFormattingTest.cs`
  (cross-context combination of attribute + named settings — this is the Mixed class's purpose).
- Test data `.../test/data/CodeFormatter/CSharp/AlignArguments/Mixed/AlignArgumentsInAttribute_NamedArguments_MultiLine.cs`
  and `.cs.gold`, following the existing naming pattern and the `<seealso>` convention in both
  directions.

Test: `TestAlignArgumentsInAttribute_NamedArguments_MultiLine`, settings
`ALIGN_ATTRIBUTE_ARGUMENTS = AlignPosition.SpaceBeforeComma` and `ALIGN_ARGUMENTS_NAMED = true`.

Test data must contain (Jurassic Park values, 2-space indent):

1. Two or more multi-line MIXED attribute lists (positional arguments first, named argument on
   its own line), so the comma gaps could align across the lists if the rule still fired.
2. A pair of purely positional multi-line attributes, proving comma alignment still works.
3. A fully-named multi-line attribute list, proving the named rule still pads it.

Expected gold: positional pairs align their commas; the mixed lists show NO comma padding and
the named `Name =` gaps padded by the named rule. Write the gold by hand from the rule
descriptions — do not generate it from the formatter output without checking it against the
expected behavior.

### 3.3 — Verify existing gold files are unaffected

Run the full suite. Expected: all 42 existing formatter tests stay green without gold changes
(the current Mixed attribute test covers only fully-named and single-line lists). If a gold diff
appears anyway, review whether the new output is correct per the stand-back design before
touching the gold file — never regenerate blindly.

### 3.4 — Docs

- `AGENTS.md`: update the Mixed test count (`Cross-context combinations (6)` -> `(7)`), the
  formatter-test count (`The 42 formatter tests` -> `43`), and the named-argument stand-back
  description so it explicitly includes attribute named arguments.
- `README.md` + `DEVELOPERS.md`: extend the "comma alignment rules stand back for calls that use
  named arguments" sentences to say attribute lists are covered the same way.
- Keep changes to these sentences surgical; do not restructure the docs.

Verification: full test run green including the new test; docs counts match reality.

## Step Plan

| Status | Step | Verification |
|--------|------|---------------|
| [x] | 1. Single ancestor walk in `GetContainingBlockOffset` | `dotnet test` green, gold untouched |
| [x] | 2. Early `ICSharpArgument or IPropertyAssignment` check in named rule `Calculate` | `dotnet test` green, gold untouched |
| [x] | 3.1 Stand-back check for `IAttribute.PropertyAssignments` in comma + first-argument builders | compile + step 3.2 test |
| [x] | 3.2 New `TestAlignArgumentsInAttribute_NamedArguments_MultiLine` + hand-written gold | new test passes, old tests green |
| [x] | 3.3 Confirm no existing gold changed | `dotnet test` green, `git diff` shows no `.gold` changes |
| [x] | 3.4 Update AGENTS.md / README.md / DEVELOPERS.md counts + stand-back sentences | docs match code, test run green |

## Notes for the executor

- One full `dotnet test` run takes ~15-30 min. Run it once per step, after the step's edits are
  complete; for 3.1 + 3.2 one combined run is acceptable since 3.1 alone has no new assertion.
- CRLF gotcha with the edit tool: see reference.md ("Constraints") — use byte-exact python
  edits if a multi-line match fails.
- Mark steps `[~]` / `[x]` in this table while working; record findings that change later steps
  in this file, below this line.

## Findings / deviations

- SDK verified by decompiling 2025.3.0.1 (ilspycmd, `JetBrains.ReSharper.Psi.CSharp.dll`):
  `IAttribute.PropertyAssignments` is `TreeNodeCollection<IPropertyAssignment>` — a `readonly struct`
  implementing `IList<T>` with a `Count` property (also `PropertyAssignmentsEnumerable`). A cheap
  `is IAttribute { PropertyAssignments.Count: > 0 }` pattern works for step 3.1; accessing the struct
  property does not allocate.
- Engine fact, decompiled `IntAlignStage.DoProcessBlockWithIntAlign` / `ProcessLines`
  (JetBrains.ReSharper.Psi.dll): a column only accepts an interval whose line number is exactly the
  column's last line number + 1, and blocks are split at lines with no intervals (blank and comment
  lines break groups). Consequence: same-argument-index comma gaps of multi-line attribute lists can
  never land on consecutive lines, so multi-line mixed lists show NO comma padding even in the current
  buggy build. The bug is observable only when two mixed attributes carry same-index comma gaps on
  consecutive lines — i.e. stacked single-line mixed lists, or a single-line positional attribute
  directly above a mixed list's positional line.
- Step 3.2 consequence (design deviation): the test data keeps the plan's multi-line mixed lists
  (positional first, named arguments on their own lines — padded by the named rule, standing back from
  comma alignment) but ADDS a pair of stacked single-line mixed lists so the test actually fails
  without the step 3.1 fix, and proves the fix observably. The "purely positional multi-line pair" is
  realized as single-line positional attributes stacked above a multi-line positional attribute, which
  is the only layout where comma alignment provably still fires; a fully multi-line positional pair can
  never align per the engine facts above.
- Step 3.2 gold verified by hand AND by falsification: with the 3.1 stand-back temporarily removed,
  the new test fails exactly on scenario 2 (stacked single-line mixed lists gain comma padding:
  [TestCase("Triceratops"       , "Blue" , park = ...)]); with the fix restored it is green again.
  The test is therefore a real gate on the fix, not a shape-only test.
- Scenario 2 of the new gold also shows the platform built-in argument-start alignment from
  INT_ALIGN_INVOCATIONS on the second mixed line; no Alignator rule keys those gaps - stand-back means
  Alignator adds no padding, not byte-identical lines.
- The gold auto-creation mechanism writes the actual output to a gitignored .cs.tmp on the no-gold run
  (useful for future test-data work).

