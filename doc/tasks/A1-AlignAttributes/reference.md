# A1-AlignAttributes - Reference

## Goal

Add alignment support for attribute arguments, specifically for TestCase attributes and attributes in general.

### Acceptance Criteria
- Attributes with multiple arguments can be aligned after commas
- Attributes with multiple arguments can be aligned with closing parenthesis/bracket
- Works for all attributes, not just TestCase
- Settings configurable via .editorconfig and UI
- Tests pass for new functionality
- Existing alignment functionality remains unaffected

## User Request

The user provided examples showing three formats:

1. **Current (no alignment):**
```csharp
[TestCase("Tyrannosaurus", "Rex")]
```

2. **AfterComma alignment:**
```csharp
[TestCase("Tyrannosaurus",      "Rex")]
```

3. **AfterComma + Closing Parenthesis alignment:**
```csharp
[TestCase("Tyrannosaurus",      "Rex"   )]
```

## Relevant Files

### Core Files
| File | Purpose |
|------|---------|
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProvider.cs` | Core formatter logic - registers alignment rules |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignPosition.cs` | Enum defining alignment positions (DoNotChange, SpaceBeforeComma, SpaceAfterComma) |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs` | Settings definitions with .editorconfig aliases |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs` | Settings UI page with live preview |
| `src/dotnet/Alignator/Helpers/NodeHelpers.cs` | Helper methods for PSI tree traversal |

### Test Files
| File | Purpose |
|------|---------|
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/CSharpQuirkyFormattingTest.cs` | Formatter tests |
| `src/dotnet/Alignator.Tests/test/data/CodeFormatter/CSharp/AlignArguments/*.cs` | Test input files |
| `src/dotnet/Alignator.Tests/test/data/CodeFormatter/CSharp/AlignArguments/*.cs.gold` | Expected output files |

## Current Behavior

The plugin currently supports alignment for:
- Constructor calls (`new Type(...)`)
- Function invocations (`Foo(...)`)

Both with two alignment modes:
- `SpaceBeforeComma`: Aligns the comma tokens themselves
- `SpaceAfterComma`: Aligns the space after commas, making arguments align

Settings:
- `INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR`
- `INT_ALIGN_ARGUMENTS_IN_FUNCTION`

## Required Changes

### 1. New Alignment Positions (Optional)
The current `AlignPosition` enum has:
- `DoNotChange`
- `SpaceBeforeComma`
- `SpaceAfterComma`

For the closing parenthesis/bracket alignment, we might need:
- `SpaceBeforeClosingParen` or similar

However, looking at the examples more carefully, the alignment is:
- After the comma (SpaceAfterComma)
- Before the closing paren (new position)

### 2. New Settings
Need new settings key for attributes:
- `INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE`

With .editorconfig alias:
- `csharp_alignator_align_arguments_in_attribute`

### 3. New Alignment Rules
Need to add rules for:
- `IAttribute` nodes with argument lists
- Support both SpaceAfterComma and SpaceBeforeClosingParen modes

### 4. PSI Node Types
Attributes in ReSharper PSI:
- `IAttribute` - the attribute itself
- `IAttributeArgumentList` - the argument list
- `IAttributeArgument` - individual arguments

Need to identify the correct node types for attribute argument alignment.

## Architecture Pattern

Following the existing pattern in `AlignArgumentsProvider.cs`:

1. Create a new `ArgumentAlignVariant` for attributes
2. Add methods like `INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceAfterComma()`
3. Register them in `Initialize()`
4. The `BuildIntAlignArgumentsRule` method can likely be reused with different predicates

### Parent Predicate
For attributes, need to match:
- `IAttributeArgumentList` where parent is `IAttribute`

### Statement Predicate
Attributes can appear in:
- Attribute lists on type declarations
- Attribute lists on method declarations
- Attribute lists on property declarations
- Attribute lists on field declarations
- And others

Need to determine the appropriate statement predicate or use a different scoping mechanism.

## Constraints

1. **Scoping**: Alignment should be scoped to consecutive attributes or within a single attribute list
2. **Node Types**: Must use correct ReSharper PSI node types for attributes
3. **Backward Compatibility**: Existing settings and rules must continue to work
4. **Performance**: Alignment rules should not significantly impact formatting performance

## Open Questions

1. Should we add a separate setting for closing parenthesis alignment, or combine it with AfterComma?
   - **Decision**: Add as separate enum value `SpaceBeforeClosingParen` for flexibility
2. Should the alignment be scoped to individual attributes or across multiple consecutive attributes?
   - **Decision**: Scope to attributes within the same attribute list (e.g., multiple attributes on same method)
3. What's the correct PSI node type for attribute argument lists?
   - **Likely**: `IAttributeArgumentList` which is a child of `IAttribute`
4. Should we support alignment for attribute named arguments (`[TestCase(Name = "Rex")]`)?
   - **Decision**: Initially focus on positional arguments; named arguments can be added later

## Research Needed

- [ ] Verify PSI node types for attributes in ReSharper
- [ ] Check if `IAttributeArgumentList` is the correct type and has the expected structure
- [ ] Understand how attributes are structured in the PSI tree
- [ ] Determine if `BuildIntAlignArgumentsRule` can be reused or if a new method is needed
