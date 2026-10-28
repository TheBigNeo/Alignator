using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     Registers the alignment rules for arguments of invocation statements (<c>ALIGN_FUNCTION_ARGUMENTS*</c>).
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
}
