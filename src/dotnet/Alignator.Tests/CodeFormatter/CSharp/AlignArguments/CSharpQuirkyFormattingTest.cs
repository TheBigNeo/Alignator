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
}
