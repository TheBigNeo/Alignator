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

        ALIGN_ATTRIBUTE_ARGUMENTS_SpaceBeforeComma();
        ALIGN_ATTRIBUTE_ARGUMENTS_SpaceAfterComma();
        ALIGN_ATTRIBUTE_ARGUMENTS_FIRST_SpaceBeforeParenthese();
        ALIGN_ATTRIBUTE_ARGUMENTS_FIRST_SpaceAfterParenthese();
        ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN_SpaceBeforeClosingParen();
    }

    /// <summary>
    ///     Aligns arguments of attributes by padding before the comma,
    ///     so the commas share one column per argument index. Attribute arguments
    ///     are direct children of the <see cref="IAttribute"/> node - there is no argument list node.
    /// </summary>
    /// <example>
    ///     <code>
    ///     [TestCase("Tyrannosaurus"     , "Rex")]
    ///     [TestCase("Moros"             , "Moros")]
    ///     [TestCase("Pachycephalosaurus", "Pachy")]
    ///     </code>
    /// </example>
    private void ALIGN_ATTRIBUTE_ARGUMENTS_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS),
                SettingsKey: x => x.ALIGN_ATTRIBUTE_ARGUMENTS,
                ParentPredicate: node => node is IAttribute,
                StatementPredicate: node => node is IAttributeSectionList,
                OffsetProvider: AttributeSectionListOffset,
                AlignKeyGroup: "Attr",
                Position: AlignPosition.SpaceBeforeComma
            ));

    /// <summary>
    ///     Aligns arguments of attributes by padding after the comma,
    ///     so the arguments following them share one column per argument index. Attribute arguments
    ///     are direct children of the <see cref="IAttribute"/> node - there is no argument list node.
    /// </summary>
    /// <example>
    ///     <code>
    ///     [TestCase("Tyrannosaurus",      "Rex")]
    ///     [TestCase("Moros",              "Moros")]
    ///     [TestCase("Pachycephalosaurus", "Pachy")]
    ///     </code>
    /// </example>
    private void ALIGN_ATTRIBUTE_ARGUMENTS_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS),
                SettingsKey: x => x.ALIGN_ATTRIBUTE_ARGUMENTS,
                ParentPredicate: node => node is IAttribute,
                StatementPredicate: node => node is IAttributeSectionList,
                OffsetProvider: AttributeSectionListOffset,
                AlignKeyGroup: "Attr",
                Position: AlignPosition.SpaceAfterComma
            ));

    /// <summary>
    ///     Aligns the first argument of attributes by padding before the opening parenthesis,
    ///     so the parentheses - and with them the first arguments - share one column. The
    ///     remaining arguments are aligned by the
    ///     <see cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS"/> comma setting -
    ///     the two settings are independent and can be enabled at the same time.
    /// </summary>
    private void ALIGN_ATTRIBUTE_ARGUMENTS_FIRST_SpaceBeforeParenthese()
        => BuildIntAlignFirstArgumentRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST),
                SettingsKey: x => x.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST,
                ParentPredicate: node => node is IAttribute,
                StatementPredicate: node => node is IAttributeSectionList,
                OffsetProvider: AttributeSectionListOffset,
                AlignKeyGroup: "Attr",
                FirstArgument: FirstArgumentPosition.SpaceBeforeParenthese
            ));

    /// <summary>
    ///     Aligns the first argument of attributes by padding after the opening parenthesis,
    ///     so the first arguments share one column. The remaining arguments are aligned by the
    ///     <see cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS"/> comma setting -
    ///     the two settings are independent and can be enabled at the same time.
    /// </summary>
    private void ALIGN_ATTRIBUTE_ARGUMENTS_FIRST_SpaceAfterParenthese()
        => BuildIntAlignFirstArgumentRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST),
                SettingsKey: x => x.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST,
                ParentPredicate: node => node is IAttribute,
                StatementPredicate: node => node is IAttributeSectionList,
                OffsetProvider: AttributeSectionListOffset,
                AlignKeyGroup: "Attr",
                FirstArgument: FirstArgumentPosition.SpaceAfterParenthese
            ));

    /// <summary>
    ///     Aligns closing parentheses of attributes by padding before them,
    ///     so they share one column. Controlled by a separate setting, so comma alignment
    ///     and closing parenthesis alignment can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     [TestCase("Tyrannosaurus"     , "Rex"   )]
    ///     [TestCase("Moros"             , "Moros" )]
    ///     [TestCase("Pachycephalosaurus", "Pachy")]
    ///     </code>
    /// </example>
    private void ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN_SpaceBeforeClosingParen()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN) + "_SpaceBeforeClosingParen")
           .Where(
                Left().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument),
                Right().HasType(CSharpTokenType.RPARENTH),
                Parent().Satisfies((node, _) => node.NodeOrNull is IAttribute)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // Walk up to the attribute section list (one per attributed declaration)
                        // and use its start offset as the alignment group
                        for (var n = ctx.Parent.NodeOrNull; n != null; n = n.Parent)
                        {
                            if (n is not IAttributeSectionList)
                            {
                                continue;
                            }

                            return new IntAlignOptionValue(
                                $"AttrClose$ArgBeforeCloseParen$Block{AttributeSectionListOffset(n)}",
                                AlignatorPriority
                            );
                        }

                        return null;
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Returns the document start offset of the attribute section list, scoping attribute
    ///     alignment groups to the attributes of a single declaration.
    /// </summary>
    private static long AttributeSectionListOffset(ITreeNode node)
        => node.GetDocumentStartOffset().Offset;
}
