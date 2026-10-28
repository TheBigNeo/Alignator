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
public class AttributeArgumentsFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
    protected override string RelativeTestDataPath => "CodeFormatter/CSharp/AlignArguments/Attributes";

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Attributes.AlignArgumentsInAttribute_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInAttribute_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Attributes.AlignArgumentsInAttribute_SpaceBeforeParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInAttribute_SpaceBeforeParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Attributes.AlignArgumentsInAttribute_SpaceAfterParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInAttribute_SpaceAfterParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Attributes.AlignClosingParenInAttribute"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN), true)]
    public void TestAlignClosingParenInAttribute()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Attributes.AlignArgumentsInAttribute_SpaceBeforeCommaAndClosingParen"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS),                 AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN), true)]
    public void TestAlignArgumentsInAttribute_SpaceBeforeCommaAndClosingParen()
    {
        DoNamedTest2();
    }
}
