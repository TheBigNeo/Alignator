using JetBrains.ReSharper.FeaturesTestFramework.Formatter;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments;

[TestAlignator]
[Category("Formatting"), Category("CSharp")]
[TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES),            false)]
[TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
public class CSharpQuirkyFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
    protected override string RelativeTestDataPath => "CodeFormatter/CSharp/AlignArguments";

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR), AlignPosition.SpaceBeforeComma)]
    public void TestAlignArgumentsInConstructor_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInConstructor_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION), AlignPosition.SpaceBeforeComma)]
    public void TestAlignArgumentsInFunction_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInFunction_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR), AlignPosition.SpaceBeforeComma)]
    public void TestAlignArgumentsInConstructorAssignment_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInConstructorAssignment_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInAttribute_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE), true)]
    public void TestAlignClosingParenInAttribute()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE),     AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE), true)]
    public void TestAlignArgumentsInAttribute_SpaceBeforeCommaAndClosingParen()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENT_NAMES), true)]
    public void TestAlignArgumentNames()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENT_NAMES), true)]
    public void TestAlignArgumentsInConstructor_NamedArguments_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR), AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENT_NAMES), true)]
    public void TestAlignArgumentsInConstructor_NamedArguments_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENT_NAMES), true)]
    public void TestAlignArgumentsInFunction_NamedArguments_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENT_NAMES), true)]
    public void TestAlignArgumentsInAttribute_NamedArguments_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER), AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignClosingBracesInCollectionInitializer()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER), AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceAfterCommaAndClosingBraces()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceBeforeCommaAndClosingBraces()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_DoNotChange()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER), AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_InlineNoChange()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION), true)]
    public void TestAlignAssignmentsInWithExpression()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION), true)]
    public void TestAlignAssignmentsInWithExpression_IndependentGroups()
    {
        DoNamedTest2();
    }

    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION), true)]
    public void TestAlignAssignmentsInWithExpression_SingleLineUntouched()
    {
        DoNamedTest2();
    }

    [Test]
    public void TestAlignAssignmentsInWithExpression_DoNotChange()
    {
        DoNamedTest2();
    }
}
