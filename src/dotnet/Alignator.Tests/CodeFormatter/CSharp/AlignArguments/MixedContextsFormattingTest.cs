using JetBrains.ReSharper.FeaturesTestFramework.Formatter;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments;

/// <summary>
///     Tests that combine alignment settings across contexts - e.g. the comma alignment of one
///     context together with the context-independent named-argument alignment.
/// </summary>
[TestAlignator]
[Category("Formatting"), Category("CSharp")]
[TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES),            false)]
[TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
public class MixedContextsFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
    protected override string RelativeTestDataPath => "CodeFormatter/CSharp/AlignArguments/Mixed";

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Mixed.AlignArgumentsInConstructor_NamedArguments_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED),             true)]
    public void TestAlignArgumentsInConstructor_NamedArguments_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Mixed.AlignArgumentsInConstructor_NamedArguments_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS), AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED),             true)]
    public void TestAlignArgumentsInConstructor_NamedArguments_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Mixed.AlignArgumentsInFunction_NamedArguments_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED),         true)]
    public void TestAlignArgumentsInFunction_NamedArguments_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Mixed.AlignArgumentsInAttribute_NamedArguments_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS), AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED),           true)]
    public void TestAlignArgumentsInAttribute_NamedArguments_SpaceBeforeComma()
    {
        DoNamedTest2();
    }
}
