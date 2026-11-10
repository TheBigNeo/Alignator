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
public class FunctionArgumentsFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
    protected override string RelativeTestDataPath => "CodeFormatter/CSharp/AlignArguments/Function";

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function.AlignArgumentsInFunction_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    public void TestAlignArgumentsInFunction_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function.AlignArgumentsInFunction_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInFunction_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function.AlignArgumentsInFunction_SpaceBeforeParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInFunction_SpaceBeforeParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function.AlignArgumentsInFunction_SpaceAfterParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInFunction_SpaceAfterParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function.AlignArgumentsInFunction_SpaceBeforeParentheseAndCommaDoNotChange"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInFunction_SpaceBeforeParentheseAndCommaDoNotChange()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function.AlignArgumentsInFunction_SpaceAfterParentheseAndSpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),                 AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST),           FirstArgumentPosition.SpaceAfterParenthese)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN),    true)]
    public void TestAlignArgumentsInFunction_SpaceAfterParentheseAndSpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function.AlignArgumentsInFunction_SpaceBeforeParentheseAndSpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),             AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST),       FirstArgumentPosition.SpaceBeforeParenthese)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN), true)]
    public void TestAlignArgumentsInFunction_SpaceBeforeParentheseAndSpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function.AlignArgumentsInFunction_ClosingParen"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN), true)]
    public void TestAlignArgumentsInFunction_ClosingParen()
    {
        DoNamedTest2();
    }
}
