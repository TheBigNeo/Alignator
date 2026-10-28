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
public class CollectionInitializerFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
    protected override string RelativeTestDataPath => "CodeFormatter/CSharp/AlignArguments/CollectionInitializer";

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.CollectionInitializer.AlignKeyValuesInCollectionInitializer_SpaceAfterComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceAfterComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.CollectionInitializer.AlignKeyValuesInCollectionInitializer_SpaceBeforeComma"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceBeforeComma()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.CollectionInitializer.AlignClosingBracesInCollectionInitializer"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES),              true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignClosingBracesInCollectionInitializer()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.CollectionInitializer.AlignKeyValuesInCollectionInitializer_SpaceAfterCommaAndClosingBraces"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES),              true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceAfterCommaAndClosingBraces()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.CollectionInitializer.AlignKeyValuesInCollectionInitializer_SpaceBeforeCommaAndClosingBraces"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceBeforeComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES),              true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_SpaceBeforeCommaAndClosingBraces()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.CollectionInitializer.AlignKeyValuesInCollectionInitializer_DoNotChange"/>
    [Test]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_DoNotChange()
    {
        DoNamedTest2();
    }

    /// <seealso cref="TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.CollectionInitializer.AlignKeyValuesInCollectionInitializer_InlineNoChange"/>
    [Test]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES),                 AlignPosition.SpaceAfterComma)]
    [TestSetting(typeof(AlignatorSettingsKey), nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES),              true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.SPACE_WITHIN_SINGLE_LINE_ARRAY_INITIALIZER_BRACES), false)]
    public void TestAlignKeyValuesInCollectionInitializer_InlineNoChange()
    {
        DoNamedTest2();
    }
}
