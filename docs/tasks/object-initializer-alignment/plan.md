# Plan — Object Initializer Alignment in Collections

Three freely combinable settings that vertically align object initializers
(`new X() { .. }`) inside one collection expression (`[ .. ]`) or collection
initializer (`new List<T> { .. }`), one entry per line:

| # | Setting (key) | Type | Effect |
|---|---------------|------|--------|
| 1 | `ALIGN_OBJECT_INITIALIZER_OPENING_BRACES` | `OpeningBracePosition`: `DoNotChange` (default), `SpaceBeforeBrace`, `SpaceAfterBrace` | `SpaceBeforeBrace` aligns the `{` of every entry into one column (user example 1); `SpaceAfterBrace` leaves the braces and aligns the first member starts |
| 2 | `ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS` | bool, default false | aligns the k-th member start and the k-th `=` across entries (user example 2) |
| 3 | `ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES` | bool, default false | aligns the `}` of every entry's object initializer into one column (user example 3) |

Scope of every alignment group is the enclosing collection (expression or
initializer) — entries of different collections never align. Entries must start their
own line and span a single line; everything else stays untouched. All settings ride on
the built-in `int_align_invocations` switch like every Alignator rule.

Rule registration (new provider `ObjectInitializerArgumentsProvider`, pattern of
`WithExpressionAssignmentsProvider`):

- `_SpaceBeforeBrace` / `_SpaceAfterBrace`: gap between the creation's argument list
  end and `{` (setting 1), key `ObjInit$BeforeOpenBrace$Coll{offset}`
- `_BeforeEq`: gap before the `=` of an `IMemberInitializer` (setting 2), key
  `ObjInit$BeforeEq$Coll{offset}`
- `_AfterComma`: gap between a member comma and the next member (setting 2), key
  `ObjInit$AfterComma$Coll{offset}`
- `_BeforeClosingBrace`: gap before the `}` of the object initializer (setting 3),
  key `ObjInit$BeforeCloseBrace$Coll{offset}`

All four use one shared key per collection without an element index: the engine aligns
the k-th same-key gap of each line as one column group (proven by the dictionary
rules). Keys are disjoint from all existing Alignator keys, so the three settings
combine freely with each other and with the existing context settings.

## Step Plan

| Status | Step | Verification |
|--------|------|--------------|
| [ ] | 1. Add `OpeningBracePosition` enum (`DoNotChange`, `SpaceBeforeBrace`, `SpaceAfterBrace`) in `src/dotnet/Alignator/Psi/CodeStyle/Formatting/OpeningBracePosition.cs`, styled like `FirstArgumentPosition.cs` | `dotnet build` of `Alignator.csproj` compiles |
| [ ] | 2. Add the three settings keys in `AlignatorSettingsKey.cs`: new `#region Object initializer` between Function and With expression, `[SettingsEntry]` + `[EditorConfigEntryAlias]` (`alignator_align_object_initializer_*`); extend the context list in the class remarks | Build compiles; `AllSettingsAtOnceCoverageTest` now fails with exactly the three new keys — visible gate until step 9 |
| [ ] | 3. Add `ObjectInitializerArgumentsProvider.cs` with the four rules and the shared entry-qualification helper (`IObjectCreationExpression` incl. `new()` with non-empty `IObjectInitializer`, direct child of `ICollectionExpression` or `ICollectionInitializer`; own line, single line); confirm the PSI gap-parent assumptions from the reference first | Build compiles; existing formatter tests all green (no rule fires, all settings default off); PSI structure confirmed via debugger |
| [ ] | 4. Add the three `ItemFor` entries in `AlignatorSettingsSchema.DescribePart` at alphabetical position (between FUNCTION and WITH_EXPRESSION) with Jurassic Park collection-expression previews and `IntAlignFixup` | Build compiles; existing tests green |
| [ ] | 5. Add test class `ObjectInitializerFormattingTest` + test data folder `ObjectInitializer/`; tests for setting 1: `_SpaceBeforeBrace` (user example 1), `_SpaceAfterBrace`, `_DoNotChange` | New tests green via the Gradle test task |
| [ ] | 6. Add tests for setting 2: assignments only, and combined with `SpaceBeforeBrace` (user example 2) | New tests green |
| [ ] | 7. Add tests for setting 3: closing braces only, combined 1+2, and 1+2+3 (user example 3) | New tests green |
| [ ] | 8. Add edge-case tests: single-line collection untouched, single entry untouched, multi-line entry ignored, `}` on its own line untouched, entries without initializer between initialized ones, target-typed `new()`, named ctor arguments inside entries, two collections align independently, nested collection expressions, `new List<T> { .. }` container | New tests green; each `.gold` shows the intended columns |
| [ ] | 9. Extend `MixedContextsFormattingTest.TestAllSettingsAtOnce` with the three `[TestSetting]`s and extend the showcase data/gold | `AllSettingsAtOnceCoverageTest` green again; showcase test green |
| [ ] | 10. Update `README.md` (settings table, `.editorconfig` examples with the `OpeningBracePosition` values, example sections) and the `AGENTS.md` rule/settings tables | Build still green (test data is compiled); docs reviewed against the final key names |

## Verification baseline

- Full test suite via the Gradle build (`build.gradle.kts` runs the .NET tests).
- Every step ends with its own verification before the next step starts (Ralph loop,
  `PlanRules.md` Phase 4).
