# A1-AlignAttributes - Implementation Plan

## Goal

Add alignment support for attribute arguments (e.g., `[TestCase("a", "b")]`) with configurable alignment positions.

## Approach

Following the existing pattern for constructor and function argument alignment, we will:

1. Extend the `AlignPosition` enum to support closing parenthesis alignment
2. Add new settings key for attribute alignment
3. Add alignment rules for attribute argument lists
4. Add tests for the new functionality
5. Update the settings UI

### Key Design Decisions

**Decision 1: New Alignment Position**
- Add `SpaceBeforeClosingParen` to `AlignPosition` enum
- This allows aligning the closing parenthesis/bracket of attributes

**Decision 2: Separate Setting for Attributes**
- Create `INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE` setting
- Separate from constructor/function alignment to allow independent configuration

**Decision 3: Reuse Existing Infrastructure**
- Leverage `BuildIntAlignArgumentsRule` method with attribute-specific predicates
- Minimal code duplication

**Decision 4: Scoping**
- Align attribute arguments within the same attribute list
- Use the attribute's parent node for grouping (similar to block offset for statements)

---

## Step Plan

| Status | Step | Verification |
|--------|------|--------------|
| [ ] | 1. Add `SpaceBeforeClosingParen` to `AlignPosition` enum | Compile, ensure enum is valid |
| [ ] | 2. Add `INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE` setting to `AlignatorSettingsKey.cs` | Compile, check .editorconfig alias |
| [ ] | 3. Add methods in `AlignArgumentsProvider.cs` for attribute alignment (SpaceAfterComma and SpaceBeforeClosingParen) | Compile, verify method signatures |
| [ ] | 4. Register new rules in `Initialize()` method | Compile, check rule registration |
| [ ] | 5. Update `AlignatorSettingsSchema.cs` to add UI controls for new setting | Compile, verify UI preview works |
| [ ] | 6. Add test class methods in `CSharpQuirkyFormattingTest.cs` for new alignment modes | Tests compile |
| [ ] | 7. Create test input files for attribute alignment | Files exist and are valid C# |
| [ ] | 8. Create test gold files with expected output | Files exist and match expected format |
| [ ] | 9. Run all tests to verify existing functionality not broken | All tests pass |
| [ ] | 10. Run new tests to verify attribute alignment works | New tests pass |

---

## Step Details

### Step 1: Extend AlignPosition Enum

**File:** `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignPosition.cs`

**Change:**
```csharp
public enum AlignPosition
{
    DoNotChange,
    SpaceBeforeComma,
    SpaceAfterComma,
    SpaceBeforeClosingParen,  // NEW
}
```

**Verification:** Project compiles successfully

---

### Step 2: Add Setting Key

**File:** `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs`

**Change:** Add new property:
```csharp
[EditorConfigEntryAlias("alignator_align_arguments_in_attribute", EditorConfigAliasType.LanguageSpecificStandard)]
[SettingsEntry(AlignPosition.DoNotChange, "Align arguments in attribute")]
public AlignPosition INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE;
```

**Verification:** 
- Project compiles
- Setting appears in generated configuration

---

### Step 3: Add Alignment Rule Methods

**File:** `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProvider.cs`

**Changes:** Add two new methods following the existing pattern:

```csharp
private void INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceAfterComma()
    => BuildIntAlignArgumentsRule(
        new ArgumentAlignVariant(
            SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE),
            SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE,
            ParentPredicate: node => node is IArgumentList {Parent: IAttribute},
            StatementPredicate: node => node is IAttribute,
            AlignKeyGroup: "Attr",
            Position: AlignPosition.SpaceAfterComma
        ));

private void INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceBeforeClosingParen()
    => BuildIntAlignArgumentsRule(
        new ArgumentAlignVariant(
            SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE) + "_ClosingParen",
            SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE,
            ParentPredicate: node => node is IArgumentList {Parent: IAttribute},
            StatementPredicate: node => node is IAttribute,
            AlignKeyGroup: "AttrClose",
            Position: AlignPosition.SpaceBeforeClosingParen
        ));
```

**Note:** Need to verify that `IAttribute` has an `IArgumentList` child. Based on ReSharper PSI, attributes use `IAttributeArgumentList` which contains arguments. Need to adjust predicates accordingly.

**Revised predicates:**
```csharp
ParentPredicate: node => node is IAttributeArgumentList,
StatementPredicate: node => node.Parent is IAttribute,
```

**Verification:** Project compiles

---

### Step 4: Register Rules

**File:** `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProvider.cs`

**Change:** Add to `Initialize()` method:
```csharp
protected override void Initialize()
{
    base.Initialize();

    INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceBeforeComma();
    INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceAfterComma();
    INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_Assignment_SpaceBeforeComma();
    INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_Assignment_SpaceAfterComma();
    INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceBeforeComma();
    INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceAfterComma();
    
    // NEW:
    INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceAfterComma();
    INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceBeforeClosingParen();
}
```

**Verification:** Project compiles

---

### Step 5: Update Settings UI

**File:** `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs`

**Change:** Add new `ItemFor` in `DescribePart` method:

```csharp
builder
   .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR, ...)
   .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION, ...)
   // NEW:
   .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE,
            """
            [TestCase("Änderung", "Anderung")]
            [TestCase("Café", "Cafe")]
            [TestCase("Zürich", "Zurich")]
            [TestFixture]
            public void MyTest() { }
            """,
            parseType: PreviewParseType.Member,
            previewFixup: IntAlignFixup)
   ;
```

**Verification:** 
- Project compiles
- UI shows new setting in Settings > Code Editing > C# > Code Style > Others > Alignator align arguments

---

### Step 6: Add Test Methods

**File:** `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/CSharpQuirkyFormattingTest.cs`

**Change:** Add two new test methods:

```csharp
[Test]
[TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE), AlignPosition.SpaceAfterComma)]
public void TestAlignArgumentsInAttribute_SpaceAfterComma()
{
    DoNamedTest2();
}

[Test]
[TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE), AlignPosition.SpaceBeforeClosingParen)]
public void TestAlignArgumentsInAttribute_SpaceBeforeClosingParen()
{
    DoNamedTest2();
}
```

**Verification:** Tests compile

---

### Step 7-8: Create Test Data Files

**Files to create:**
- `test/data/CodeFormatter/CSharp/AlignArguments/AlignArgumentsInAttribute_SpaceAfterComma.cs`
- `test/data/CodeFormatter/CSharp/AlignArguments/AlignArgumentsInAttribute_SpaceAfterComma.cs.gold`
- `test/data/CodeFormatter/CSharp/AlignArguments/AlignArgumentsInAttribute_SpaceBeforeClosingParen.cs`
- `test/data/CodeFormatter/CSharp/AlignArguments/AlignArgumentsInAttribute_SpaceBeforeClosingParen.cs.gold`

**Example content for SpaceAfterComma:**

Input file:
```csharp
[TestCase("Änderung", "Anderung")]
[TestCase("Café", "Cafe")]
[TestCase("Zürich", "Zurich")]
[TestCase("La Première", "La Premiere")]
public void MyTest() { }
```

Gold file:
```csharp
[TestCase("Änderung",     "Anderung")]
[TestCase("Café",        "Cafe")]
[TestCase("Zürich",      "Zurich")]
[TestCase("La Première", "La Premiere")]
public void MyTest() { }
```

**Example content for SpaceBeforeClosingParen:**

Input file:
```csharp
[TestCase("Änderung"    , "Anderung")]
[TestCase("Café"        , "Cafe")]
[TestCase("Zürich"      , "Zurich")]
public void MyTest() { }
```

Gold file:
```csharp
[TestCase("Änderung"    , "Anderung"   )]
[TestCase("Café"        , "Cafe"       )]
[TestCase("Zürich"      , "Zurich"     )]
public void MyTest() { }
```

**Verification:** Files are valid C# and match expected formatting

---

### Step 9-10: Run Tests

**Commands:**
```bash
# Run all existing tests
./gradlew testDotNet

# Or run specific test class
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj --filter "FullyQualifiedName~CSharpQuirkyFormattingTest"
```

**Verification:** All tests pass (existing + new)

---

## Potential Issues and Mitigations

### Issue 1: Incorrect PSI Node Types
**Risk:** Using wrong node types for attributes
**Mitigation:** Check ReSharper documentation or existing code that handles attributes. The correct types are likely `IAttribute` and `IAttributeArgumentList`.

### Issue 2: BuildIntAlignArgumentsRule Not Working for Attributes
**Risk:** The existing `BuildIntAlignArgumentsRule` method may not work for attribute argument lists
**Mitigation:** May need to create a separate method for attributes or adjust the predicates significantly.

### Issue 3: Scoping Problems
**Risk:** Alignment may not scope correctly for attributes
**Mitigation:** Use attribute's parent (the attributed declaration) for scoping, similar to how block offset is used for statements.

### Issue 4: Closing Parenthesis Alignment Complexity
**Risk:** Aligning closing parenthesis may be more complex than comma alignment
**Mitigation:** Start with SpaceAfterComma first, verify it works, then add SpaceBeforeClosingParen.

---

## Contingency Plan

If the above approach doesn't work:

1. **Fallback:** Create a separate alignment rule class specifically for attributes
2. **Simplify:** Only implement SpaceAfterComma first, defer SpaceBeforeClosingParen to a separate task
3. **Research:** Investigate how ReSharper's IntelliJ-based formatter handles attribute alignment

---

## Success Criteria

- [ ] New setting `INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE` is configurable via .editorconfig
- [ ] New setting appears in Rider UI under Alignator section
- [ ] Attributes with multiple arguments align after commas when configured
- [ ] Attributes with multiple arguments align closing parenthesis when configured
- [ ] All existing tests continue to pass
- [ ] New tests pass
- [ ] No performance regression
