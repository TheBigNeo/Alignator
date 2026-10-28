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
public class WithExpressionFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
    protected override string RelativeTestDataPath => "CodeFormatter/CSharp/AlignArguments/WithExpression";

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.WithExpression.AlignAssignmentsInWithExpression"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_WITH_EXPRESSION_ASSIGNMENTS), true)]
    public void TestAlignAssignmentsInWithExpression()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.WithExpression.AlignAssignmentsInWithExpression_IndependentGroups"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_WITH_EXPRESSION_ASSIGNMENTS), true)]
    public void TestAlignAssignmentsInWithExpression_IndependentGroups()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.WithExpression.AlignAssignmentsInWithExpression_SingleLineUntouched"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_WITH_EXPRESSION_ASSIGNMENTS), true)]
    public void TestAlignAssignmentsInWithExpression_SingleLineUntouched()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.WithExpression.AlignAssignmentsInWithExpression_DoNotChange"/>
    [Test]
    public void TestAlignAssignmentsInWithExpression_DoNotChange()
    {
        DoNamedTest2();
    }
}
