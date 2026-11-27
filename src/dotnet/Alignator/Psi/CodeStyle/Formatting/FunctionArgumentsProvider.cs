using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using TBN.Alignator.Helpers;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     Registers the alignment rules for arguments of invocation statements (<c>ALIGN_FUNCTION_ARGUMENTS*</c>),
///     including the closing-parenthesis variant.
/// </summary>
[Language(typeof(CSharpLanguage))]
public class FunctionArgumentsProvider : AlignArgumentsProviderBase
{
    public FunctionArgumentsProvider(ISettingsSchema           settingsSchema,
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

        ALIGN_FUNCTION_ARGUMENTS_CommaPositions();
        ALIGN_FUNCTION_ARGUMENTS_FIRST_ParenPositions();
        ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN_SpaceBeforeClosingParen();
    }

    /// <summary>
    ///     Aligns arguments of invocation statements around the commas:
    ///     padding before the comma makes the commas share one column per argument index,
    ///     padding after it makes the arguments following them share one column per argument index.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.FeedDinosaurs("T-Rex"      , "Carnivore");
    ///     Caller.FeedDinosaurs("Triceratops", "Herbi");
    ///     </code>
    ///     <code>
    ///     Caller.FeedDinosaurs("T-Rex",       "Carnivore");
    ///     Caller.FeedDinosaurs("Triceratops", "Herbi");
    ///     </code>
    /// </example>
    private void ALIGN_FUNCTION_ARGUMENTS_CommaPositions()
    {
        foreach (var position in new[] {AlignPosition.SpaceBeforeComma, AlignPosition.SpaceAfterComma})
        {
            BuildIntAlignArgumentsRule(
                new ArgumentAlignVariant(
                    SettingName: nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),
                    SettingsKey: x => x.ALIGN_FUNCTION_ARGUMENTS,
                    ParentPredicate: node => node is IArgumentList {Parent: IInvocationExpression},
                    StatementPredicate: node => node is IExpressionStatement,
                    OffsetProvider: null,
                    AlignKeyGroup: "Func",
                    Position: position
                ));
        }
    }

    /// <summary>
    ///     Aligns the first argument of invocation statements at the opening parenthesis:
    ///     padding before the opening parenthesis makes the parentheses - and with them the first arguments - share one column,
    ///     padding after it makes the first arguments share one column behind the untouched parentheses.
    ///     The remaining arguments are aligned by the <see cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS"/>
    ///     comma setting - the two settings are independent and can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.FeedDinosaurs               ("T-Rex",      "Raptor");
    ///     Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor");
    ///     </code>
    ///     <code>
    ///     Caller.FeedDinosaurs(                "T-Rex",      "Raptor");
    ///     Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor");
    ///     </code>
    /// </example>
    private void ALIGN_FUNCTION_ARGUMENTS_FIRST_ParenPositions()
    {
        foreach (var position in new[] {FirstArgumentPosition.SpaceBeforeParenthese, FirstArgumentPosition.SpaceAfterParenthese})
        {
            BuildIntAlignFirstArgumentRule(
                new ArgumentAlignVariant(
                    SettingName: nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST),
                    SettingsKey: x => x.ALIGN_FUNCTION_ARGUMENTS_FIRST,
                    ParentPredicate: node => node is IArgumentList {Parent: IInvocationExpression},
                    StatementPredicate: node => node is IExpressionStatement,
                    OffsetProvider: null,
                    AlignKeyGroup: "Func",
                    FirstArgument: position
                ));
        }
    }

    /// <summary>
    ///     Aligns closing parentheses of invocation statements by padding before them, so they share one column.
    ///     Controlled by a separate setting,
    ///     so comma alignment and closing parenthesis alignment can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.FeedDinosaurs("T-Rex", "Raptor", "Moros", "Triceratops"  );
    ///     Caller.FeedDinosaurs("Veloci", "Baryo", "Moso", "Dilophosaurus");
    ///     </code>
    /// </example>
    private void ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN_SpaceBeforeClosingParen()
        => BuildIntAlignClosingParenRule(
            new ClosingParenVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN),
                SettingsKey: x => x.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN,
                ParentPredicate: node => node is IInvocationExpression,
                LeftPredicate: node => node is ICSharpArgument or IArgumentList,
                StatementPredicate: node => node is IExpressionStatement,
                OffsetProvider: node => node.GetContainingBlockOffset(),
                ArgumentsProvider: node => (node as IInvocationExpression)?.ArgumentList?.Arguments,
                AlignKeyGroup: "FuncClose"
            ));
}
