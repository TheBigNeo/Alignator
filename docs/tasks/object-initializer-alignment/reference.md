# Reference — Object Initializer Alignment in Collections

Task: three new Alignator settings that align object initializers (`new X() { .. }`)
inside collection expressions `[ .. ]` and collection initializers
(`new List<T> { .. }`), one entry per line.

## Goal and acceptance criteria

**Goal (one sentence):** Add three freely combinable settings that vertically align the
opening braces, the member assignments, and the closing braces of object initializers
inside one collection (expression or initializer).

**Acceptance criteria:**

1. Setting 1 — opening braces: with `SpaceBeforeBrace` the `{` of every entry shares
   one column (User example 1); with `SpaceAfterBrace` the first member starts share one
   column; default `DoNotChange` pads nothing.
2. Setting 2 — member assignments: the k-th member's start and `=` share one column
   across entries (User example 2, typically combined with Setting 1).
3. Setting 3 — closing braces: the `}` of every entry's object initializer shares one
   column (User example 3, typically combined with Settings 1 and 2).
4. Alignment groups are scoped per collection: entries of different collections never
   align with each other.
5. Entries not starting their own line, multi-line entries, and fully single-line
   collections stay untouched.
6. All settings require the built-in `int_align_invocations` switch, like every
   Alignator rule.
7. All new and existing formatter tests pass; `AllSettingsAtOnceCoverageTest` stays
   green (new keys are exercised by the showcase test).

## Relevant files and key symbols

| File | Key symbols |
|------|-------------|
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProviderBase.cs` | `AlignatorPriority`, `StartsOwnLineAfterToken`, `ContainsLineBreak`, `TryGetArguments`, `FindAlignmentBlockOffset` |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/WithExpressionAssignmentsProvider.cs` | `ALIGN_WITH_EXPRESSION_ASSIGNMENTS_BeforeEq` — per-construct scoped rule pattern, `StartsOwnLineAfterToken` usage |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/CollectionInitializerArgumentsProvider.cs` | `TryGetAlignableInitializer` — per-initializer scoping and entry qualification pattern; `ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES_BeforeClosingBrace` |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs` | key naming scheme (`ALIGN_<CONTEXT>_<FAMILY>[_<VARIANT>]`), `[SettingsEntry]`, `[EditorConfigEntryAlias]`, regions |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs` | `DescribePart` `ItemFor` entries, `IntAlignFixup`, alphabetical order |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignPosition.cs`, `FirstArgumentPosition.cs` | enum style for the new `OpeningBracePosition` |
| `src/dotnet/Alignator/Helpers/NodeHelpers.cs` | `GetContainingBlockOffset` (block scoping — not used here, per-collection scope instead) |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/*FormattingTest.cs` | test class pattern, `[TestSetting]`, `DoNamedTest2()` |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/MixedContextsFormattingTest.cs` | `TestAllSettingsAtOnce` — must set every new key |
| `src/dotnet/Alignator.Tests/test/data/CodeFormatter/CSharp/AlignArguments/<Context>/` | `.cs` / `.cs.gold` pairs, `<seealso>` links both ways |

## Current behavior and why the change is needed

The formatter aligns constructor/function/attribute arguments, named arguments,
dictionary-style entries of collection initializers, and with-expression assignments.
Object-creation entries inside a collection expression or collection initializer —
`new X() { A = .., B = .. }` — are never aligned: no rule matches the gaps around the
entry's object initializer braces or between its member assignments. The three new
settings fill that gap.

## Constraints, conventions, gotchas

- SDK 2025.3 exposes `ICollectionExpression` (C# 12 `[ .. ]`), `IObjectInitializer`
  (`Initializers` getter), and `IMemberInitializer` — verified present in
  `JetBrains.ReSharper.Psi.CSharp.dll`.
- Key naming: `ALIGN_OBJECT_INITIALIZER_*`, new region alphabetically between Function
  and With expression in `AlignatorSettingsKey.cs`; same alphabetical position for the
  `ItemFor` entries in `AlignatorSettingsSchema.cs`.
- Alignment keys must be disjoint from existing ones; use the `ObjInit$...$Coll{offset}`
  prefix. One shared key per collection without an element index: the engine aligns the
  k-th same-key gap of each line as one column group (pattern proven by the dictionary
  rules).
- Dictionary rules and the new rules are disjoint: dictionary entries are
  `ICollectionElementInitializer` with 2+ arguments; new rules match
  `IObjectCreationExpression` entries with a non-empty `IObjectInitializer`.
- Constructor comma rules may still fire on `new X(args)` inside entries (block scope) —
  existing behavior, cover with an edge-case test.
- Test data: 2-space indent (project `.editorconfig`), Jurassic Park names, `<seealso>`
  crefs both directions, files are compiled via `Compile Include`.
- Example values in docs and settings previews use Jurassic Park dinosaur names.
- CHANGELOG.md must not be touched.

## Open questions / risky assumptions

1. PSI gap ownership: the gap between `)` and `{` probably has parent
   `IObjectCreationExpression` (like the first-argument paren gaps); the gap before `}`
   probably has parent `IObjectInitializer`; member gaps have parents
   `IMemberInitializer` / `IObjectInitializer`. Must be confirmed with the PSI
   debugger/inspector during Step 3 before finalizing the `Where` clauses.
2. Whether the k-th-gap grouping handles lines with differing member counts (1 vs. 2
   members) exactly as the examples show — covered by combination tests in Steps 6-8.
3. Test framework parses C# 12 collection expressions (SDK 2025.3, `LangVersion=Latest`)
   — confirmed by config, verified for real in Step 5.
