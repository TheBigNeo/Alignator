using JetBrains.ReSharper.FeaturesTestFramework.Formatter;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments;

/// <summary>
///     Tests the alignment of object initializers inside collections
///     (<see cref="ObjectInitializerArgumentsProvider"/>):
///     opening braces, member assignments and closing braces of entries
///     that start their own line and span a single line.
/// </summary>
[TestAlignator]
[Category("Formatting"), Category("CSharp")]
[TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES),            false)]
[TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
public class ObjectInitializerFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
    protected override string RelativeTestDataPath => "CodeFormatter/CSharp/AlignArguments/ObjectInitializer";

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.AlignOpeningBraces_SpaceBeforeBrace"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    public void TestAlignOpeningBraces_SpaceBeforeBrace()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.AlignOpeningBraces_SpaceAfterBrace"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceAfterBrace)]
    public void TestAlignOpeningBraces_SpaceAfterBrace()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.AlignOpeningBraces_DoNotChange"/>
    [Test]
    public void TestAlignOpeningBraces_DoNotChange()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.AlignAssignments"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS), true)]
    public void TestAlignAssignments()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.AlignAssignments_WithOpeningBraces"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    public void TestAlignAssignments_WithOpeningBraces()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.AlignClosingBraces"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES), true)]
    public void TestAlignClosingBraces()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.AlignAllObjectInitializerSettings"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestAlignAllObjectInitializerSettings()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.SingleLineCollectionUntouched"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestSingleLineCollectionUntouched()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.SingleEntryUntouched"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestSingleEntryUntouched()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.MultiLineEntryIgnored"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestMultiLineEntryIgnored()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.ClosingBraceOnOwnLineUntouched"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestClosingBraceOnOwnLineUntouched()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.EntriesWithoutInitializer"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestEntriesWithoutInitializer()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.TargetTypedNew"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestTargetTypedNew()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.NamedCtorArguments"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestNamedCtorArguments()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.IndependentCollections"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestIndependentCollections()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.NestedCollectionExpressions"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestNestedCollectionExpressions()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer.CollectionInitializerContainer"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES), OpeningBracePosition.SpaceBeforeBrace)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS),      true)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES),  true)]
    public void TestCollectionInitializerContainer()
    {
        DoNamedTest2();
    }
}
