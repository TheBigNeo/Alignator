# A1-AlignAttributes Task

## Overview

This task implements alignment support for attribute arguments in the Alignator plugin, allowing users to align attribute parameters similar to how constructor and function arguments are aligned.

## User Request

The user wants to align TestCase attribute arguments (and attributes in general) with the following formatting options:

### Current Format
```csharp
[TestCase("Änderung", "Anderung")]
```

### Option 1: AfterComma Alignment
```csharp
[TestCase("Änderung"                    , "Anderung")]
```

### Option 2: AfterComma + Closing Parenthesis Alignment
```csharp
[TestCase("Änderung"                    , "Anderung"                   )]
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

1. **New Enum Value**: Add `SpaceBeforeClosingParen` to support closing parenthesis alignment
2. **Separate Setting**: Create `INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE` for independent configuration
3. **Reuse Infrastructure**: Leverage existing `BuildIntAlignArgumentsRule` with attribute-specific predicates
4. **Scoping**: Align within the same attribute list (attributes on the same declaration)

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
