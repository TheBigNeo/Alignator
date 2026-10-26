# A1-AlignAttributes Task

## Overview

This task implements alignment support for attribute arguments in the Alignator plugin, allowing users to align attribute parameters similar to how constructor and function arguments are aligned.

## User Request

The user wants to align TestCase attribute arguments (and attributes in general) with the following formatting options:

### Current Format
```csharp
[TestCase("Änderung",                    "Anderung")]
[TestCase("Café",                        "Cafe")]
[TestCase("Naïve",                       "Naive")]
[TestCase("Zürich",                      "Zurich")]
[TestCase("La Première",                 "La Premiere")]
[TestCase("Schnittplätze_2026",         "Schnittplatze_2026")]
[TestCase("Schnittplätze_2026",          "Schnittplatze_2026")]
[TestCase("àáâãäåèéêëìíîïòóôõöùúûüýÿñç", "aaaaaaeeeeiiiiooooouuuuyync")]
[TestCase("ÀÁÂÃÄÅÈÉÊËÌÍÎÏÒÓÔÕÖÙÚÛÜÝÑÇ",  "AAAAAAEEEEIIIIOOOOOUUUUYNC")]
```

### SpaceAfterComma
```csharp
[TestCase("Änderung",                    "Anderung")]
[TestCase("Café",                        "Cafe")]
[TestCase("Naïve",                       "Naive")]
[TestCase("Zürich",                      "Zurich")]
[TestCase("La Première",                 "La Premiere")]
[TestCase("Schnittplätze_2026",         "Schnittplatze_2026")]
[TestCase("Schnittplätze_2026",          "Schnittplatze_2026")]
[TestCase("àáâãäåèéêëìíîïòóôõöùúûüýÿñç", "aaaaaaeeeeiiiiooooouuuuyync")]
[TestCase("ÀÁÂÃÄÅÈÉÊËÌÍÎÏÒÓÔÕÖÙÚÛÜÝÑÇ",  "AAAAAAEEEEIIIIOOOOOUUUUYNC")]
```


### SpaceBeforeComma
```csharp
[TestCase("Änderung"                    ,  "Anderung")]
[TestCase("Café"                        ,  "Cafe")]
[TestCase("Naïve"                       ,  "Naive")]
[TestCase("Zürich"                      ,  "Zurich")]
[TestCase("La Première"                 ,  "La Premiere")]
[TestCase("Schnittplätze_2026"          , "Schnittplatze_2026")]
[TestCase("Schnittplätze_2026"          ,  "Schnittplatze_2026")]
[TestCase("àáâãäåèéêëìíîïòóôõöùúûüýÿñç" ,  "aaaaaaeeeeiiiiooooouuuuyync")]
[TestCase("ÀÁÂÃÄÅÈÉÊËÌÍÎÏÒÓÔÕÖÙÚÛÜÝÑÇ"  ,  "AAAAAAEEEEIIIIOOOOOUUUUYNC")]
```

### Option 2: AfterComma + Closing Parenthesis Alignment
```csharp
[TestCase("Änderung"                    , "Anderung"                   )]
[TestCase("Café"                        , "Cafe"                       )]
[TestCase("Naïve"                       , "Naive"                      )]
[TestCase("Zürich"                      , "Zurich"                     )]
[TestCase("La Première"                 , "La Premiere"                )]
[TestCase("Schnittplätze_2026"          , "Schnittplatze_2026"         )]
[TestCase("Schnittplätze_2026"          , "Schnittplatze_2026"         )]
[TestCase("àáâãäåèéêëìíîïòóôõöùúûüýÿñç" , "aaaaaaeeeeiiiiooooouuuuyync")]
[TestCase("ÀÁÂÃÄÅÈÉÊËÌÍÎÏÒÓÔÕÖÙÚÛÜÝÑÇ"  , "AAAAAAEEEEIIIIOOOOOUUUUYNC" )]
```

## Files Created

| File | Purpose |
|------|---------|
| `reference.md` | Context, constraints, open questions, research needed |
| `plan.md` | Detailed implementation plan with step-by-step instructions |

## Quick Start

To begin implementation, follow the steps in `plan.md` in order:

1. Extend `AlignPosition` enum
2. Add new setting key
3. Add alignment rule methods
4. Register rules
5. Update settings UI
6. Add tests
7. Create test data
8. Verify all tests pass

## Key Design Decisions

1. **Two Independent Settings (Option A)**: Comma alignment and closing parenthesis alignment are controlled by **separate settings**, so both can be active simultaneously
   - `INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE`: Controls comma alignment (`DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`)
   - `INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE`: Controls closing parenthesis alignment (`true`/`false` or its own `AlignPosition`)
2. **New Enum Value**: Add `SpaceBeforeClosingParen` to `AlignPosition` to support closing parenthesis alignment
3. **Reuse Infrastructure**: Leverage existing `BuildIntAlignArgumentsRule` with attribute-specific predicates
4. **Scoping**: Align within the same attribute list (attributes on the same declaration)

## Expected Behavior

The user can independently enable/disable each alignment type via `.editorconfig`:

```ini
[*.cs]
# Comma alignment: DoNotChange | SpaceBeforeComma | SpaceAfterComma
csharp_alignator_align_arguments_in_attribute = space_after_comma
# Closing parenthesis alignment: true | false
csharp_alignator_align_closing_paren_in_attribute = true
```

With both enabled, the result is the "Option 2" format shown above: arguments aligned after commas AND closing parentheses aligned in the same column.

## Dependencies

- ReSharper SDK 2025.3.0.1
- .NET Framework 4.7.2
- Existing Alignator infrastructure (AlignArgumentsProvider, AlignPosition, etc.)

## Estimated Effort

- **Low Risk**: Steps 1-5 (enum extension, settings, rule registration) - ~2-4 hours
- **Medium Risk**: Steps 6-8 (tests, PSI node verification) - ~4-8 hours
- **Total**: ~6-12 hours

## Risks

1. **PSI Node Types**: May need to verify correct node types for attributes
2. **BuildIntAlignArgumentsRule**: May not work directly for attribute argument lists
3. **Scoping**: May need adjustments for proper attribute alignment grouping

## Next Steps

1. Review `reference.md` and `plan.md`
2. Verify the approach with stakeholders
3. Begin implementation with Step 1 (extend enum)
4. Proceed sequentially through the plan, verifying each step

## Current Implementation State

**Completed:**
- `SpaceBeforeClosingParen` added to `AlignPosition` enum
- `INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE` setting added to `AlignatorSettingsKey.cs` (comma alignment only)
- Alignment rule method `INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceAfterComma()` registered in `AlignArgumentsProvider.cs`
- Settings UI preview added in `AlignatorSettingsSchema.cs`
- Test methods and test data files created

**Outstanding (Option A):**
- Add `INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE` setting (independent of comma alignment setting)
- Register the closing parenthesis alignment rule using the new setting
- Fix PSI node matching: current predicates (`IArgumentList {Parent: IAttribute}` + `IClassLikeDeclaration` statement) do not match attribute argument lists — alignment rules fire for constructors/functions but not for attributes
- Update tests to cover both settings independently and combined
- Verify existing tests still pass (6 passing)
