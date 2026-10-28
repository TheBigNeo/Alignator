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

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructor_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    public void TestAlignArgumentsInConstructor_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructor_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInConstructor_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInConstructor_SpaceBeforeParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructor_SpaceAfterParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInConstructor_SpaceAfterParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructor_SpaceBeforeParentheseAndCommaDoNotChange"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInConstructor_SpaceBeforeParentheseAndCommaDoNotChange()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructor_SpaceAfterParentheseAndSpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),         AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInConstructor_SpaceAfterParentheseAndSpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInFunction_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    public void TestAlignArgumentsInFunction_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInFunction_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInFunction_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInFunction_SpaceBeforeParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInFunction_SpaceBeforeParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInFunction_SpaceAfterParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInFunction_SpaceAfterParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInFunction_SpaceAfterParentheseAndSpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),         AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInFunction_SpaceAfterParentheseAndSpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructorAssignment_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    public void TestAlignArgumentsInConstructorAssignment_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructorAssignment_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInConstructorAssignment_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructorAssignment_SpaceAfterParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInConstructorAssignment_SpaceAfterParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInAttribute_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInAttribute_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInAttribute_SpaceBeforeParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInAttribute_SpaceBeforeParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInAttribute_SpaceAfterParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInAttribute_SpaceAfterParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignClosingParenInAttribute"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN), true)]
    public void TestAlignClosingParenInAttribute()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInAttribute_SpaceBeforeCommaAndClosingParen"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS),     AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN), true)]
    public void TestAlignArgumentsInAttribute_SpaceBeforeCommaAndClosingParen()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentNames"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED), true)]
    public void TestAlignArgumentNames()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructor_NamedArguments_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED),             true)]
    public void TestAlignArgumentsInConstructor_NamedArguments_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInConstructor_NamedArguments_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED),             true)]
    public void TestAlignArgumentsInConstructor_NamedArguments_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInFunction_NamedArguments_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED),         true)]
    public void TestAlignArgumentsInFunction_NamedArguments_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignArgumentsInAttribute_NamedArguments_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED),           true)]
    public void TestAlignArgumentsInAttribute_NamedArguments_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignKeyValuesInCollectionInitializer_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignKeyValuesInCollectionInitializer_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignClosingBracesInCollectionInitializer"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES),              true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignClosingBracesInCollectionInitializer()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignKeyValuesInCollectionInitializer_SpaceAfterCommaAndClosingBraces"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES),              true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceAfterCommaAndClosingBraces()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignKeyValuesInCollectionInitializer_SpaceBeforeCommaAndClosingBraces"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES),              true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceBeforeCommaAndClosingBraces()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignKeyValuesInCollectionInitializer_DoNotChange"/>
    [Test]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_DoNotChange()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignKeyValuesInCollectionInitializer_InlineNoChange"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES),              true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_InlineNoChange()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignAssignmentsInWithExpression"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_WITH_EXPRESSION_ASSIGNMENTS), true)]
    public void TestAlignAssignmentsInWithExpression()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignAssignmentsInWithExpression_IndependentGroups"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_WITH_EXPRESSION_ASSIGNMENTS), true)]
    public void TestAlignAssignmentsInWithExpression_IndependentGroups()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignAssignmentsInWithExpression_SingleLineUntouched"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_WITH_EXPRESSION_ASSIGNMENTS), true)]
    public void TestAlignAssignmentsInWithExpression_SingleLineUntouched()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.AlignAssignmentsInWithExpression_DoNotChange"/>
    [Test]
    public void TestAlignAssignmentsInWithExpression_DoNotChange()
    {
        DoNamedTest2();
    }
}
