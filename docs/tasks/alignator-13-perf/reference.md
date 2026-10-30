# Reference — Alignator #13: Formatter Hot-Path Performance

Branch: `Alignator-13-Cleanup`. Read `AGENTS.md` and `docs/PlanRules.md` first.

## Goal and acceptance criteria

Goal: keep the Alignator formatter plugin cheap on CPU in its hot path — the `Where`/`Calculate`
callbacks that run per whitespace gap / per node on every C# format run — and close one
correctness gap found during the review.

Acceptance criteria:

- All tests green after every step: `dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj`.
- Steps 1 and 2 change no gold file (pure hot-path work, behavior identical).
- Step 3 is a deliberate behavior change: attribute lists that use named arguments stand back
  from comma/paren alignment; verified by one new test with its own gold file.
- Docs (AGENTS.md, README.md, DEVELOPERS.md) match the code after step 3 (test counts).

## Already done in this branch (do not redo)

- Setting `ALIGN_ARGUMENTS_NAMED_ALL` was deleted: key in `AlignatorSettingsKey.cs`, the
  `IndentingRule` in `NamedArgumentsProvider.cs`, its helpers (`CalculateValueColumnOffset`,
  `NamePrefixLength`, `NextSolidSibling`), the settings-UI preview entry, the test
  `TestAlignArgumentNamesAll` and its data files, and all doc mentions. The plugin now registers
  23 `IntAlignRule`s and has no `IndentingRule` left. 42 formatter tests + 1 coverage guard,
  all green (exit code 0).
- Perf fix #2 (replace `whitespace.GetText().IndexOf('\n')` with a `NEW_LINE` token-type check)
  was REJECTED after SDK verification. Do not implement it:
  - `WhitespaceBase.GetText()` returns a cached string field — no allocation.
  - `NEW_LINE` is a strict superset: it also covers `\r`, `\u2028`, `\u2029`, `\u0085`, which
    `IndexOf('\n')` misses — the change would alter behavior in CR-only files for zero gain.

## Engine facts (verified against the decompiled 2025.3.0.1 ReSharper SDK)

- `Where` predicates run during rule matching for every gap/node in the rule's bucket, BEFORE any
  setting is read. The `SwitchOnExternalKey` setting gate applies only to `Calculate`. Cheap
  type checks belong in `Where`; anything walking subtrees or ancestors belongs in `Calculate`.
- In ReSharper C# PSI, a named attribute argument `[Test(Name = "x")]` is an `IPropertyAssignment`
  (parsed by `CSharpParser.ParseAttributeArguments` -> `ParsePropertyAssignment`) — NOT an
  `ICSharpArgument`. `IAttribute.Arguments` returns only positional arguments; the named ones are
  a separate `IAttribute.PropertyAssignments` collection. The `=` gap's parent is the
  `IPropertyAssignment`. Call-side named arguments (`name:`) are `ICSharpArgument` with children
  `[name, COLON, value]`.
- `IPropertyAssignment` also appears in object initializers (`new T { A = 1 }`) — the reason
  `IsInsideAttributeArgument` must keep its ancestor walk.
- `IBlock` implementers: `Block`, `ChameleonBlock` (lazily parsed statement bodies).
  `ICSharpTypeDeclaration` implementers: `ClassDeclaration`, `StructDeclaration`,
  `InterfaceDeclaration`, `RecordDeclaration`, `EnumDeclaration`, `DelegateDeclaration`,
  `ExtensionDeclaration`. C# grammar forbids type declarations inside blocks, so no
  `ICSharpTypeDeclaration` can sit between a start node and an `IBlock` above it — a single
  ancestor walk checking both (block first) is equivalent to the current two walks.
- The alignment engine aligns only gaps without line breaks, and only the k-th same-key gap of
  consecutive lines as one column group.

## Relevant files and key symbols

| File | Key symbols | Note |
|------|-------------|------|
| `src/dotnet/Alignator/Helpers/NodeHelpers.cs` | `GetContainingBlockOffset` | Two sequential ancestor walks today; called per gap via `FindAlignmentBlockOffset` when a rule's `OffsetProvider` is null (constructor + function rules). Attribute rules use `AttributeSectionListOffset` and never call it. |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProviderBase.cs` | `BuildIntAlignArgumentsRule`, `BuildIntAlignFirstArgumentRule`, `BuildIntAlignClosingParenRule`, `TryGetArguments`, `UsesNamedArguments`, `FindAlignmentBlockOffset` | `TryGetArguments(IAttribute)` returns only positional args; `UsesNamedArguments` therefore never sees attribute named arguments — the root cause of step 3. |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/NamedArgumentsProvider.cs` | `ALIGN_ARGUMENTS_NAMED_AfterNameColon` (Calculate), `FindNameToken`, `IsInsideAttributeArgument`, `GapAfterNameTokenContainsLineBreak` | `Where` matches Left `COLON`/`EQ` + Right expression — so `Calculate` is reached for every `=` and ternary-`:` gap in the document when the setting is on. |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AttributeArgumentsProvider.cs` | closing-paren rule, `AttributeSectionListOffset` | Closing-paren rule's `LeftPredicate` is `ICSharpArgument`; fully-named attribute lists can never match it (left of the gap is inside an `IPropertyAssignment`). Behavior is consistent with stand-back — document, do not change. |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/MixedContextsFormattingTest.cs` | `TestAllSettingsAtOnce`, attribute+named tests | Pattern for the new step-3 test. |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/AllSettingsAtOnceCoverageTest.cs` | reflection guard | Fails if an `AlignatorSettingsKey` field has no `TestSetting` on `TestAllSettingsAtOnce`. No key changes planned. |
| test data | `src/dotnet/Alignator.Tests/test/data/CodeFormatter/CSharp/AlignArguments/{Attributes,Mixed,Named}/` | `.cs` inputs are compiled (wildcard `Compile Include`) and carry `<seealso>` links in both directions; gold files use 2-space indent (test `.editorconfig`). |

## Current behavior and why each change is needed

1. `GetContainingBlockOffset` walks the ancestor chain twice whenever no `IBlock` exists
   (field/property initializers, top-level statements): once for `IBlock` to the root, once for
   `ICSharpTypeDeclaration`. That is per matched gap, inside `Calculate`, on every format run.
2. The named-argument rule's `Calculate` runs `FindNameToken` (recursive subtree scan, for an
   ordinary assignment the whole LHS) and then, on the `=` path, `IsInsideAttributeArgument`
   (ancestor walk to root) for every `=`-gap in the document — including `x = 1` statements,
   `Foo(y = 1)`, ternaries, object initializers, with expressions. All of these are rejected
   anyway; the check order is just expensive. Both accepted shapes have an argument-shaped gap
   parent (`ICSharpArgument` for `name:`, `IPropertyAssignment` for attribute `Name =`).
3. Mixed attribute lists (`[TestCase("Tyrannosaurus", "Rex", park = "Jurassic Park")]`, written
   across lines): the comma rule's `Where` matches the positional arguments' comma gaps (they are
   `ICSharpArgument`), and `UsesNamedArguments` cannot see `park` (it is an `IPropertyAssignment`,
   not in `IAttribute.Arguments`), so the comma rule pads those lines while the named rule pads
   the `=` gaps on the same lines. Call lists with named arguments stand back entirely — the
   documented design is "no line is padded twice". Fully-named attribute lists are unaffected
   today only because the comma rule's `Where` (`Left is ICSharpArgument`) never matches there.

## Constraints, conventions and gotchas

- AGENTS.md: answer-first brevity, surgical changes, **DO NOT COMMIT**, **do not touch
  CHANGELOG.md**. Jurassic Park names for all test data values (`Tyrannosaurus`, `Velociraptor`,
  `Triceratops`, `Blue`, ...).
- Test data files: 2-space indent, `<seealso>` crefs in both directions, CS0219 suppressed.
- New test classes need `[TestAlignator]`, the two `CSharpFormatSettingsKey` base settings
  (`WRAP_LINES=false`, `INT_ALIGN_INVOCATIONS=true`) — copy the header of an existing test class.
- A full `dotnet test` run takes roughly 15-30 minutes; it is the per-step verification, do not
  run it after every file edit.
- Repo files are CRLF. The edit tool failed to match multi-line strings in CRLF files in a
  previous session (single-line edits worked). If an edit reports "not found", switch to a
  byte-exact python script (open `'rb'`, assert the pattern occurs once, replace, write `'wb'`)
  or single-line edits — do not retry the same edit blindly.
- Count references to update after step 3: AGENTS.md repo structure (`MixedContextsFormattingTest
  # Cross-context combinations (6)`) and Key Files (`The 42 formatter tests`), plus the
  named-argument stand-back sentences in README.md/DEVELOPERS.md/AGENTS.md if they gain the
  attribute case.

## Open questions and risky assumptions

- Step 3 assumes stand-back is the intended design for attribute named arguments (documented
  "no line is padded twice"; call contexts already stand back). The alternative — keep comma
  alignment for mixed attribute lists — was rejected for consistency.
- The exact shape of `IAttribute.PropertyAssignments` (collection type, count accessor) must be
  confirmed in the SDK before coding step 3; the plan says "verify, then use".
- Existing gold files are expected to be unaffected by step 3 (the current Mixed attribute test
  covers only fully-named and single-line lists). If `dotnet test` still shows a gold diff, review
  whether the new behavior is correct before updating the gold — never regenerate blindly.
