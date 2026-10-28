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

        ALIGN_FUNCTION_ARGUMENTS_SpaceBeforeComma();
        ALIGN_FUNCTION_ARGUMENTS_SpaceAfterComma();
        ALIGN_FUNCTION_ARGUMENTS_FIRST_SpaceBeforeParenthese();
        ALIGN_FUNCTION_ARGUMENTS_FIRST_SpaceAfterParenthese();
        ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN_SpaceBeforeClosingParen();
    }

    /// <summary>
    ///     Aligns arguments of invocation statements by padding before the comma,
    ///     so the commas share one column per argument index.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.F1("T-Rex"      , "Carnivore");
    ///     Caller.F1("Triceratops", "Herbi");
    ///     </code>
    /// </example>
    private void ALIGN_FUNCTION_ARGUMENTS_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),
                SettingsKey: x => x.ALIGN_FUNCTION_ARGUMENTS,
                ParentPredicate: node => node is IArgumentList {Parent: IInvocationExpression},
                StatementPredicate: node => node is IExpressionStatement,
                OffsetProvider: null,
                AlignKeyGroup: "Func",
                Position: AlignPosition.SpaceBeforeComma
            ));

    /// <summary>
    ///     Aligns arguments of invocation statements by padding after the comma,
    ///     so the arguments following them share one column per argument index.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.F1("T-Rex",       "Carnivore");
    ///     Caller.F1("Triceratops", "Herbi");
    ///     </code>
    /// </example>
    private void ALIGN_FUNCTION_ARGUMENTS_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS),
                SettingsKey: x => x.ALIGN_FUNCTION_ARGUMENTS,
                ParentPredicate: node => node is IArgumentList {Parent: IInvocationExpression},
                StatementPredicate: node => node is IExpressionStatement,
                OffsetProvider: null,
                AlignKeyGroup: "Func",
                Position: AlignPosition.SpaceAfterComma
            ));

    /// <summary>
    ///     Aligns the first argument of invocation statements by padding before the opening
    ///     parenthesis, so the parentheses - and with them the first arguments - share one
    ///     column. The remaining arguments are aligned by the
    ///     <see cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS"/> comma setting -
    ///     the two settings are independent and can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.Function1(               "T-Rex",      "Raptor");
    ///     Caller.ThatIsADifferentFunction("Gallimimus", "Indor");
    ///     </code>
    /// </example>
    private void ALIGN_FUNCTION_ARGUMENTS_FIRST_SpaceBeforeParenthese()
        => BuildIntAlignFirstArgumentRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST),
                SettingsKey: x => x.ALIGN_FUNCTION_ARGUMENTS_FIRST,
                ParentPredicate: node => node is IArgumentList {Parent: IInvocationExpression},
                StatementPredicate: node => node is IExpressionStatement,
                OffsetProvider: null,
                AlignKeyGroup: "Func",
                FirstArgument: FirstArgumentPosition.SpaceBeforeParenthese
            ));

    /// <summary>
    ///     Aligns the first argument of invocation statements by padding after the opening
    ///     parenthesis, so the first arguments share one column. The remaining arguments are
    ///     aligned by the <see cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS"/>
    ///     comma setting - the two settings are independent and can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.Function1(                "T-Rex",      "Raptor");
    ///     Caller.ThatIsADifferentFunction("Gallimimus", "Indor");
    ///     </code>
    /// </example>
    private void ALIGN_FUNCTION_ARGUMENTS_FIRST_SpaceAfterParenthese()
        => BuildIntAlignFirstArgumentRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST),
                SettingsKey: x => x.ALIGN_FUNCTION_ARGUMENTS_FIRST,
                ParentPredicate: node => node is IArgumentList {Parent: IInvocationExpression},
                StatementPredicate: node => node is IExpressionStatement,
                OffsetProvider: null,
                AlignKeyGroup: "Func",
                FirstArgument: FirstArgumentPosition.SpaceAfterParenthese
            ));

    /// <summary>
    ///     Aligns closing parentheses of invocation statements by padding before them,
    ///     so they share one column. Controlled by a separate setting, so comma alignment
    ///     and closing parenthesis alignment can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.Function1("T-Rex", "Raptor", "Moros", "Triceratops"  );
    ///     Caller.Function2("Veloci", "Baryo", "Moso", "Dilophosaurus");
    ///     </code>
    /// </example>
    private void ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN_SpaceBeforeClosingParen()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN) + "_SpaceBeforeClosingParen")
           .Where(
                Left().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument or IArgumentList),
                Right().HasType(CSharpTokenType.RPARENTH),
                Parent().Satisfies((node, _) => node.NodeOrNull is IInvocationExpression)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the invocation: the closing parenthesis is a
                        // sibling of the argument list, not a child of it
                        if (ctx.Parent.NodeOrNull is not IInvocationExpression invocation)
                        {
                            return null;
                        }

                        // An empty argument list has no argument the closing parenthesis could align with
                        if (invocation.ArgumentList.Arguments.Count == 0)
                        {
                            return null;
                        }

                        // A closing parenthesis wrapped onto its own line cannot be pulled onto the
                        // argument line: the gap between them contains the line break
                        if (ctx.GetLinebreaksNumber() > 0)
                        {
                            return null;
                        }

                        // Walk up to the enclosing statement and use its block offset as the
                        // alignment group, like the comma rules: the closing parentheses of
                        // consecutive invocation statements of one block share one column
                        for (var n = ctx.Parent.NodeOrNull; n != null; n = n.Parent)
                        {
                            if (n is not IExpressionStatement)
                            {
                                continue;
                            }

                            var blockOffset = n.GetContainingBlockOffset();

                            if (blockOffset == null)
                            {
                                return null;
                            }

                            return new IntAlignOptionValue(
                                $"FuncClose$ArgBeforeCloseParen$Block{blockOffset}",
                                AlignatorPriority
                            );
                        }

                        return null;
                    }
                )
            )
           .Build();
    }
}
