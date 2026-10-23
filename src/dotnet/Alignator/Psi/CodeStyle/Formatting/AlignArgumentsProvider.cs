using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle.Formatter;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using TBN.Alignator.Helpers;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

[Language(typeof(CSharpLanguage))]
public class AlignArgumentsProvider : CSharpFormatterInfoProviderPart
{
    private const int AlignatorPriority = 100_000;

    public AlignArgumentsProvider(ISettingsSchema           settingsSchema,
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

        INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceBeforeComma();
        INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceAfterComma();
        INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_Assignment_SpaceBeforeComma();
        INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_Assignment_SpaceAfterComma();
        INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceBeforeComma();
        INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceAfterComma();
    }

    /// <summary>
    ///     Aligns arguments of object creations in local variable declarations
    ///     by padding before the comma, so the commas share one column per argument index.
    /// </summary>
    /// <example>
    ///     <code>
    ///     var tRex  = new Tyrannosaurus("T-Rex"    , "Carnivore");
    ///     var trike = new Triceratops("Triceratops", "Herbivore");
    ///     </code>
    /// </example>
    private void INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR),
                SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: node => node is IDeclarationStatement,
                AlignKeyGroup: "Params",
                Position: AlignPosition.SpaceBeforeComma
            ));

    /// <summary>
    ///     Aligns arguments of object creations in local variable declarations
    ///     by padding after the comma, so the arguments following them share one column per argument index.
    /// </summary>
    /// <example>
    ///     <code>
    ///     var tRex  = new Tyrannosaurus("T-Rex",     "Carnivore");
    ///     var trike = new Triceratops("Triceratops", "Herbivore");
    ///     </code>
    /// </example>
    private void INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR),
                SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: node => node is IDeclarationStatement,
                AlignKeyGroup: "Params",
                Position: AlignPosition.SpaceAfterComma
            ));

    /// <summary>
    ///     Aligns arguments of object creations assigned to fields or properties - in assignment
    ///     statements inside a body and in class-level field/property initializers - by padding
    ///     before the comma, so the commas share one column per argument index.
    /// </summary>
    /// <example>
    ///     Assignment statements inside a body:
    ///     <code>
    ///     _tRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true);
    ///     _raptor = new Velociraptor("Raptor", "Carnivore", 150 , true);
    ///     </code>
    ///     Class-level field or property initializers:
    ///     <code>
    ///     private Tyrannosaurus _tRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000);
    ///     private Velociraptor  _raptor = new Velociraptor("Rex"   , "Carnivore", 150);
    ///     </code>
    /// </example>
    private void INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_Assignment_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR) + "_Assignment",
                SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: IsFieldOrPropertyAssignmentStatement,
                AlignKeyGroup: "ParamsAssign",
                Position: AlignPosition.SpaceBeforeComma
            ));

    /// <summary>
    ///     Aligns arguments of object creations assigned to fields or properties - in assignment
    ///     statements inside a body and in class-level field/property initializers - by padding
    ///     after the comma, so the arguments following them share one column per argument index.
    /// </summary>
    /// <example>
    ///     Assignment statements inside a body:
    ///     <code>
    ///     _tRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true);
    ///     _raptor = new Velociraptor("Raptor", "Carnivore", 150,  true);
    ///     </code>
    ///     Class-level field or property initializers:
    ///     <code>
    ///     private Tyrannosaurus _tRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true);
    ///     private Velociraptor  _raptor = new Velociraptor("Rex",    "Carnivore", 150,  true);
    ///     </code>
    /// </example>
    private void INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_Assignment_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR) + "_Assignment",
                SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: IsFieldOrPropertyAssignmentStatement,
                AlignKeyGroup: "ParamsAssign",
                Position: AlignPosition.SpaceAfterComma
            ));

    /// <summary>
    ///     Aligns arguments of invocation statements by padding before the comma,
    ///     so the commas share one column per argument index.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.F1("T-Rex"      , "Carnivore");
    ///     Caller.F1("Triceratops", "Herbivore");
    ///     </code>
    /// </example>
    private void INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION),
                SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION,
                ParentPredicate: node => node is IArgumentList {Parent: IInvocationExpression},
                StatementPredicate: node => node is IExpressionStatement,
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
    ///     Caller.F1("Triceratops", "Herbivore");
    ///     </code>
    /// </example>
    private void INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION),
                SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION,
                ParentPredicate: node => node is IArgumentList {Parent: IInvocationExpression},
                StatementPredicate: node => node is IExpressionStatement,
                AlignKeyGroup: "Func",
                Position: AlignPosition.SpaceAfterComma
            ));

    /// <summary>
    ///     Builds the shared <see cref="IntAlignRule"/> for one variant: it matches the whitespace
    ///     gap between an argument and its comma, and aligns all gaps that share the variant's
    ///     alignment key (key group, comma mode, argument index, enclosing block offset).
    /// </summary>
    private void BuildIntAlignArgumentsRule(ArgumentAlignVariant variant)
    {
        var isSpaceBefore = variant.Position == AlignPosition.SpaceBeforeComma;
        var nameSuffix    = isSpaceBefore ? "_SpaceBefore" : "_SpaceAfter";
        var commaKeyPart  = isSpaceBefore ? "ArgComma" : "ArgAfterComma";

        var leftCondition = isSpaceBefore
                                ? Left().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument)
                                : Left().HasType(CSharpTokenType.COMMA);

        var rightCondition = isSpaceBefore
                                 ? Right().HasType(CSharpTokenType.COMMA)
                                 : Right().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument);

        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(variant.SettingName + nameSuffix)
           .Where(
                leftCondition,
                rightCondition,
                Parent().Satisfies((node, _) => variant.ParentPredicate(node.NodeOrNull))
            )
           .SwitchOnExternalKey(
                variant.SettingsKey,
                When(variant.Position).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        if (ctx.Parent.NodeOrNull is not IArgumentList argList)
                        {
                            return null;
                        }

                        // Find the index of the argument adjacent to a comma
                        var argIndex  = 0;
                        var arguments = argList.Arguments;
                        for (var i = 0; i < arguments.Count; i++)
                        {
                            var sibling = isSpaceBefore ? arguments[i].NextSibling : arguments[i].PrevSibling;

                            while (sibling is IWhitespaceNode)
                            {
                                sibling = isSpaceBefore ? sibling.NextSibling : sibling.PrevSibling;
                            }

                            if (sibling is ITokenNode token && token.GetTokenType() == CSharpTokenType.COMMA)
                            {
                                argIndex = i;
                                break;
                            }
                        }

                        // Walk up to find the enclosing statement and use its block offset as the alignment group
                        for (var n = ctx.Parent.NodeOrNull; n != null; n = n.Parent)
                        {
                            if (!variant.StatementPredicate(n))
                            {
                                continue;
                            }

                            var blockOffset = n.GetContainingBlockOffset();

                            if (blockOffset == null)
                            {
                                return null;
                            }

                            return new IntAlignOptionValue(
                                $"{variant.AlignKeyGroup}${commaKeyPart}{argIndex}$Block{blockOffset}",
                                AlignatorPriority
                            );
                        }

                        return null;
                    }
                )
            )
           .Build();
    }

    /// <summary>Describes what makes each alignment variant unique; everything else is shared logic.</summary>
    private sealed record ArgumentAlignVariant(
        string                                                                        SettingName,
        System.Linq.Expressions.Expression<System.Func<AlignatorSettingsKey, object>> SettingsKey,
        System.Func<ITreeNode, bool>                                                  ParentPredicate,
        System.Func<ITreeNode, bool>                                                  StatementPredicate,
        string                                                                        AlignKeyGroup,
        AlignPosition                                                                 Position
    );

    /// <summary>
    ///     Returns <c>true</c> for the nodes that carry an object creation assigned to a field
    ///     or property: assignment expression statements and class-level field or property
    ///     declarations (initializers).
    /// </summary>
    private static bool IsFieldOrPropertyAssignmentStatement(ITreeNode node)
        => node is IExpressionStatement {Expression: IAssignmentExpression}
           or IFieldDeclaration
           or IPropertyDeclaration;
}