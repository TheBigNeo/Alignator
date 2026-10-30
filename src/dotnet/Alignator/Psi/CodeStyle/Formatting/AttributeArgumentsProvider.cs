using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     Registers the alignment rules for arguments of attributes (<c>ALIGN_ATTRIBUTE_ARGUMENTS*</c>),
///     including the closing-parenthesis variant.
/// </summary>
[Language(typeof(CSharpLanguage))]
public class AttributeArgumentsProvider : AlignArgumentsProviderBase
{
    public AttributeArgumentsProvider(ISettingsSchema           settingsSchema,
                                      ICalculatedSettingsSchema calculatedSettingsSchema,
                                      IThreading                threading,
                                      Lifetime                  lifetime)
        : base(settingsSchema,
               calculatedSettingsSchema,
               threading,
               lifetime)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();

        ALIGN_ATTRIBUTE_ARGUMENTS_CommaPositions();
        ALIGN_ATTRIBUTE_ARGUMENTS_FIRST_ParenPositions();
        ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN_SpaceBeforeClosingParen();
    }

    /// <summary>
    ///     Aligns arguments of attributes around the commas:
    ///     padding before the comma makes the commas share one column per argument index,
    ///     padding after it makes the arguments following them share one column per argument index.
    ///     Attribute arguments are direct children of the <see cref="IAttribute"/> node -
    ///     there is no argument list node.
    /// </summary>
    /// <example>
    ///     <code>
    ///     [TestCase("Tyrannosaurus"     , "Rex")]
    ///     [TestCase("Pachycephalosaurus", "Pachy")]
    ///     </code>
    ///     <code>
    ///     [TestCase("Tyrannosaurus",      "Rex")]
    ///     [TestCase("Pachycephalosaurus", "Pachy")]
    ///     </code>
    /// </example>
    private void ALIGN_ATTRIBUTE_ARGUMENTS_CommaPositions()
    {
        foreach (var position in new[] {AlignPosition.SpaceBeforeComma, AlignPosition.SpaceAfterComma})
        {
            BuildIntAlignArgumentsRule(
                new ArgumentAlignVariant(
                    SettingName: nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS),
                    SettingsKey: x => x.ALIGN_ATTRIBUTE_ARGUMENTS,
                    ParentPredicate: node => node is IAttribute,
                    StatementPredicate: node => node is IAttributeSectionList,
                    OffsetProvider: AttributeSectionListOffset,
                    AlignKeyGroup: "Attr",
                    Position: position
                ));
        }
    }

    /// <summary>
    ///     Aligns the first argument of attributes at the opening parenthesis:
    ///     padding before the opening parenthesis makes the parentheses - and with them the first arguments - share one column,
    ///     padding after it makes the first arguments share one column.
    ///     The remaining arguments are aligned by the <see cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS"/>
    ///     comma setting - the two settings are independent and can be enabled at the same time.
    /// </summary>
    private void ALIGN_ATTRIBUTE_ARGUMENTS_FIRST_ParenPositions()
    {
        foreach (var position in new[] {FirstArgumentPosition.SpaceBeforeParenthese, FirstArgumentPosition.SpaceAfterParenthese})
        {
            BuildIntAlignFirstArgumentRule(
                new ArgumentAlignVariant(
                    SettingName: nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST),
                    SettingsKey: x => x.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST,
                    ParentPredicate: node => node is IAttribute,
                    StatementPredicate: node => node is IAttributeSectionList,
                    OffsetProvider: AttributeSectionListOffset,
                    AlignKeyGroup: "Attr",
                    FirstArgument: position
                ));
        }
    }

    /// <summary>
    ///     Aligns closing parentheses of attributes by padding before them, so they share one column.
    ///     Controlled by a separate setting,
    ///     so comma alignment and closing parenthesis alignment can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     [TestCase("Tyrannosaurus"     , "Rex"   )]
    ///     [TestCase("Moros"             , "Moros" )]
    ///     [TestCase("Pachycephalosaurus", "Pachy")]
    ///     </code>
    /// </example>
    private void ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN_SpaceBeforeClosingParen()
        => BuildIntAlignClosingParenRule(
            new ClosingParenVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN),
                SettingsKey: x => x.ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN,
                ParentPredicate: node => node is IAttribute,
                LeftPredicate: node => node is ICSharpArgument,
                StatementPredicate: node => node is IAttributeSectionList,
                OffsetProvider: node => AttributeSectionListOffset(node),
                ArgumentsProvider: TryGetArguments,
                AlignKeyGroup: "AttrClose"
            ));

    /// <summary>
    ///     Returns the document start offset of the attribute section list,
    ///     scoping attribute alignment groups to the attributes of a single declaration.
    /// </summary>
    private static long AttributeSectionListOffset(ITreeNode node)
        => node.GetDocumentStartOffset().Offset;
}
