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
public class ConstructorArgumentsFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
    protected override string RelativeTestDataPath => "CodeFormatter/CSharp/AlignArguments/Constructor";

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructor_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    public void TestAlignArgumentsInConstructor_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructor_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInConstructor_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructor_SpaceBeforeParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInConstructor_SpaceBeforeParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructor_SpaceAfterParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInConstructor_SpaceAfterParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructor_SpaceBeforeParentheseAndCommaDoNotChange"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInConstructor_SpaceBeforeParentheseAndCommaDoNotChange()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructor_SpaceAfterParentheseAndSpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),         AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInConstructor_SpaceAfterParentheseAndSpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructorAssignment_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    public void TestAlignArgumentsInConstructorAssignment_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructorAssignment_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    public void TestAlignArgumentsInConstructorAssignment_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructorAssignment_SpaceAfterParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),         AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceAfterParenthese)]
    public void TestAlignArgumentsInConstructorAssignment_SpaceAfterParenthese()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Constructor.AlignArgumentsInConstructorAssignment_SpaceBeforeParenthese"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),         AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST), FirstArgumentPosition.SpaceBeforeParenthese)]
    public void TestAlignArgumentsInConstructorAssignment_SpaceBeforeParenthese()
    {
        DoNamedTest2();
    }
}
