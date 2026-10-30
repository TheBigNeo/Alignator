using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle.Formatter;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using TBN.Alignator.Helpers;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     Shared machinery of the Alignator alignment providers:
///     the rule builders for the comma and first-argument variants,
///     the tree helpers they share, and the alignment priority.
///     One concrete provider class per context derives from this base;
///     the platform aggregates all <see cref="CSharpFormatterInfoProviderPart"/> parts.
/// </summary>
public abstract class AlignArgumentsProviderBase : CSharpFormatterInfoProviderPart
{
    protected const int AlignatorPriority = 100_000;

    protected AlignArgumentsProviderBase(ISettingsSchema           settingsSchema,
                                         ICalculatedSettingsSchema calculatedSettingsSchema,
                                         IThreading                threading,
                                         Lifetime                  lifetime)
        : base(settingsSchema,
               calculatedSettingsSchema,
               threading,
               lifetime)
    {
    }

    /// <summary>
    ///     Builds the shared <see cref="IntAlignRule"/> for one variant:
    ///     it matches the whitespace gap between an argument and its comma,
    ///     and aligns all gaps that share the variant's alignment key
    ///     (key group, comma mode, argument index, enclosing block offset).
    /// </summary>
    protected void BuildIntAlignArgumentsRule(ArgumentAlignVariant variant)
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

                        // Arguments are direct children of the parent node:
                        // of an argument list for invocations and object creations,
                        // and of the attribute itself for attributes
                        var arguments = TryGetArguments(ctx.Parent.NodeOrNull);

                        if (arguments is null)
                        {
                            return null;
                        }

                        // Named-argument lists belong to the argument-names rule, which pads between the argument name
                        // and its value. Aligning commas on top of that pads the same lines twice, so this rule stands back
                        if (UsesNamedArguments(arguments))
                        {
                            return null;
                        }

                        // Find the index of the argument adjacent to a comma
                        var argIndex = 0;
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

                        var blockOffset = FindAlignmentBlockOffset(ctx.Parent.NodeOrNull, variant);

                        if (blockOffset is null)
                        {
                            return null;
                        }

                        return new IntAlignOptionValue(
                            $"{variant.AlignKeyGroup}${commaKeyPart}{argIndex}$Block{blockOffset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Builds the shared <see cref="IntAlignRule"/> for one first-argument variant:
    ///     it matches the whitespace gap before the opening parenthesis
    ///     (<c>SpaceBeforeParenthese</c> - the parentheses share one column, so the first arguments start in one column)
    ///     or the gap after the opening parenthesis
    ///     (<c>SpaceAfterParenthese</c> - the first arguments share one column behind the untouched parentheses),
    ///     and aligns all gaps that share the variant's alignment key (key group, paren mode, enclosing block offset).
    ///     The rule is keyed on the independent <c>ALIGN_*_ARGUMENTS_FIRST</c> settings,
    ///     so it combines freely with the comma alignment settings.
    /// </summary>
    protected void BuildIntAlignFirstArgumentRule(ArgumentAlignVariant variant)
    {
        var spaceBeforeParen = variant.FirstArgument == FirstArgumentPosition.SpaceBeforeParenthese;
        var nameSuffix   = spaceBeforeParen ? "_SpaceBeforeParen" : "_SpaceAfterParen";
        var parenKeyPart = spaceBeforeParen ? "ArgBeforeOpenParen" : "ArgAfterOpenParen";

        // Before the parenthesis the gap starts at the callee:
        // the invoked reference of an invocation, the type usage or the 'new' keyword of an object creation,
        // the name of an attribute.
        // After the parenthesis it starts at the '('.
        // The gap's parent node is the invocation, the object creation or the attribute itself -
        // the parenthesis is a sibling of the argument list node, not a child of it.
        var leftCondition = spaceBeforeParen
                                ? Left().Satisfies((_, _) => true)
                                : Left().HasType(CSharpTokenType.LPARENTH);

        var rightCondition = spaceBeforeParen
                                 ? Right().HasType(CSharpTokenType.LPARENTH)
                                 : Right().Satisfies((node, _) => node.NodeOrNull is IArgumentList or ICSharpArgument);

        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(variant.SettingName + nameSuffix)
           .Where(
                leftCondition,
                rightCondition,
                Parent().Satisfies((node, _) => node.NodeOrNull is IInvocationExpression or IObjectCreationExpression or IAttribute)
            )
           .SwitchOnExternalKey(
                variant.SettingsKey,
                When(variant.FirstArgument).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the call, the object creation or the attribute
                        var parent = ctx.Parent.NodeOrNull;
                        if (parent is null)
                        {
                            return null;
                        }

                        // Locate the node that carries the arguments:
                        // the argument list of an invocation or object creation, the attribute itself for attributes
                        ITreeNode argumentListNode = parent switch
                        {
                            IInvocationExpression invocation => invocation.ArgumentList,
                            IObjectCreationExpression creation => creation.ArgumentList,
                            IAttribute attribute => attribute,
                            _ => null,
                        };

                        if (argumentListNode is null)
                        {
                            return null;
                        }

                        // The variant's parent predicate scopes the rule to its construct -
                        // the same check the comma rules apply to the gap's argument list parent
                        if (!variant.ParentPredicate(argumentListNode))
                        {
                            return null;
                        }

                        var arguments = TryGetArguments(argumentListNode);

                        if (arguments is null)
                        {
                            return null;
                        }

                        // Named-argument lists belong to the argument-names rule -
                        // the paren alignment stands back entirely, like the comma alignment rules
                        if (UsesNamedArguments(arguments))
                        {
                            return null;
                        }

                        // An empty argument list has no first argument to align after the parenthesis -
                        // and the engine drops zero-length right nodes anyway
                        if (!spaceBeforeParen && arguments.Count == 0)
                        {
                            return null;
                        }

                        var blockOffset = FindAlignmentBlockOffset(parent, variant);

                        if (blockOffset is null)
                        {
                            return null;
                        }

                        return new IntAlignOptionValue(
                            $"{variant.AlignKeyGroup}${parenKeyPart}$Block{blockOffset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Builds the shared <see cref="IntAlignRule"/> for one closing-parenthesis variant:
    ///     it matches the whitespace gap between the last argument (or the empty argument list) and the closing
    ///     parenthesis, and aligns all gaps that share the variant's alignment key -
    ///     one shared key per statement block, so the closing parentheses of consecutive statements of one block
    ///     share one column.
    ///     Keyed on its own <c>ALIGN_*_CLOSING_PAREN</c> setting,
    ///     so it combines freely with the comma and first-argument settings.
    /// </summary>
    protected void BuildIntAlignClosingParenRule(ClosingParenVariant variant)
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(variant.SettingName + "_SpaceBeforeClosingParen")
           .Where(
                Left().Satisfies((node, _) => variant.LeftPredicate(node.NodeOrNull)),
                Right().HasType(CSharpTokenType.RPARENTH),
                Parent().Satisfies((node, _) => variant.ParentPredicate(node.NodeOrNull))
            )
           .SwitchOnExternalKey(
                variant.SettingsKey,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the invocation or the attribute:
                        // the closing parenthesis is a sibling of the argument list, not a child of it
                        var parent = ctx.Parent.NodeOrNull;

                        if (parent is null || !variant.ParentPredicate(parent))
                        {
                            return null;
                        }

                        // An empty argument list has no argument the closing parenthesis could align with
                        var arguments = variant.ArgumentsProvider(parent);

                        if (arguments is null || arguments.Count == 0)
                        {
                            return null;
                        }

                        // A closing parenthesis wrapped onto its own line cannot be pulled onto the argument line:
                        // the gap between them contains the line break.
                        // The engine leaves such a gap unpadded anyway (it only aligns gaps on consecutive lines),
                        // but the guard makes the intent explicit.
                        if (ctx.GetLinebreaksNumber() > 0)
                        {
                            return null;
                        }

                        // Walk up to the statement the variant scopes to and use its offset as the alignment group,
                        // like the comma rules: the closing parentheses of consecutive statements of one block
                        // share one column
                        for (var n = parent; n != null; n = n.Parent)
                        {
                            if (!variant.StatementPredicate(n))
                            {
                                continue;
                            }

                            var offset = variant.OffsetProvider(n);

                            if (offset is null)
                            {
                                return null;
                            }

                            return new IntAlignOptionValue(
                                $"{variant.AlignKeyGroup}$ArgBeforeCloseParen$Block{offset}",
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
    ///     Returns the arguments of the node that carries them -
    ///     the argument list of an invocation or object creation,
    ///     or the attribute itself for attributes, whose arguments are direct children -
    ///     or <c>null</c> for any other node.
    /// </summary>
    protected static IList<ICSharpArgument> TryGetArguments(ITreeNode node)
        => node switch
        {
            IArgumentList argumentList => argumentList.Arguments,
            IAttribute attribute       => attribute.Arguments,
            _                          => null,
        };

    /// <summary>
    ///     Returns <c>true</c> when any argument of the list is a named argument.
    ///     Such lists belong to the argument-names rule, which pads between the argument name and its value -
    ///     the comma and parenthesis rules stand back entirely,
    ///     including for the positional arguments of a mixed list,
    ///     so no line is padded twice.
    /// </summary>
    protected static bool UsesNamedArguments(IList<ICSharpArgument> arguments)
    {
        foreach (var argument in arguments)
        {
            if (argument.ArgumentName is not null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Walks up from the start node to the first statement the variant scopes to
    ///     and returns its alignment-group offset:
    ///     the variant's offset provider value when it defines one,
    ///     otherwise the offset of the enclosing block - alignment never crosses block boundaries.
    ///     Returns <c>null</c> when no such statement is found or the statement has no offset.
    /// </summary>
    protected static long? FindAlignmentBlockOffset(ITreeNode start, ArgumentAlignVariant variant)
    {
        for (var n = start; n != null; n = n.Parent)
        {
            if (!variant.StatementPredicate(n))
            {
                continue;
            }

            return variant.OffsetProvider?.Invoke(n) ?? n.GetContainingBlockOffset();
        }

        return null;
    }

    /// <summary>
    ///     Returns <c>true</c> when the node starts its own line:
    ///     the walk back over whitespace reaches a line break,
    ///     or - when an open token type is given - the token that opens the node's list
    ///     (an opening parenthesis or brace), which means the node is the first element of its list.
    ///     With an open token type, a comma ends the previous element of the list and returns <c>false</c> there -
    ///     a node in the middle of a line returns <c>false</c>.
    ///     Without an open token type the walk stops at the first non-whitespace node:
    ///     a node on the opening line of its list or in the middle of a line returns <c>false</c>.
    /// </summary>
    protected static bool StartsOwnLineAfterToken(ITreeNode node, Func<ITokenNode, bool> isOpenToken = null)
    {
        for (var n = PreviousNode(node); n != null; n = PreviousNode(n))
        {
            switch (n)
            {
                case IWhitespaceNode whitespace:
                    if (whitespace.GetText().IndexOf('\n') >= 0)
                    {
                        return true;
                    }

                    break;

                case ITokenNode token when isOpenToken is not null && isOpenToken(token):
                    return true;

                case ITokenNode token when isOpenToken is not null && token.GetTokenType() == CSharpTokenType.COMMA:
                    return false;

                default:
                    if (isOpenToken is null)
                    {
                        return false;
                    }

                    break;
            }
        }

        return false;
    }

    /// <summary>
    ///     Returns <c>true</c> when any descendant of the node is a whitespace containing a
    ///     line break - i.e. the node spans several lines.
    /// </summary>
    protected static bool ContainsLineBreak(ITreeNode node)
    {
        for (var child = node.FirstChild; child != null; child = child.NextSibling)
        {
            if (child is IWhitespaceNode whitespace && whitespace.GetText().IndexOf('\n') >= 0)
            {
                return true;
            }

            if (ContainsLineBreak(child))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Returns the node that ends right before the given node in document order, or <c>null</c>.</summary>
    protected static ITreeNode PreviousNode(ITreeNode node)
    {
        if (node.PrevSibling != null)
        {
            return node.PrevSibling;
        }

        var parent = node.Parent;
        return parent is null ? null : PreviousNode(parent);
    }

    /// <summary>Returns the node that starts right after the given node in document order, or <c>null</c>.</summary>
    protected static ITreeNode NextNode(ITreeNode node)
    {
        if (node.NextSibling != null)
        {
            return node.NextSibling;
        }

        var parent = node.Parent;
        return parent is null ? null : NextNode(parent);
    }

    /// <summary>
    ///     Describes what makes each alignment variant unique; everything else is shared logic.
    ///     A variant either aligns the arguments around the commas
    ///     (<see cref="Position"/>, the <c>ALIGN_*_ARGUMENTS</c> settings)
    ///     or the first argument at the opening parenthesis
    ///     (<see cref="FirstArgument"/>, the independent <c>ALIGN_*_ARGUMENTS_FIRST</c> settings) -
    ///     never both.
    /// </summary>
    protected sealed record ArgumentAlignVariant(
        string                   SettingName,
        Expression<Func<AlignatorSettingsKey, object>> SettingsKey,
        Func<ITreeNode, bool>    ParentPredicate,
        Func<ITreeNode, bool>    StatementPredicate,
        Func<ITreeNode, long>    OffsetProvider,
        string                   AlignKeyGroup,
        AlignPosition            Position = AlignPosition.DoNotChange,
        FirstArgumentPosition    FirstArgument = FirstArgumentPosition.DoNotChange
    );

    /// <summary>
    ///     Describes the context-specific parts of a closing-parenthesis alignment rule -
    ///     the node the gap belongs to, the node allowed left of the gap,
    ///     the statement the walk-up stops at, how that statement's alignment-group offset is computed,
    ///     and where the rule reads its arguments from for the empty-list check;
    ///     everything else is shared logic of <see cref="BuildIntAlignClosingParenRule"/>.
    /// </summary>
    protected sealed record ClosingParenVariant(
        string                                           SettingName,
        Expression<Func<AlignatorSettingsKey, object>>  SettingsKey,
        Func<ITreeNode, bool>                            ParentPredicate,
        Func<ITreeNode, bool>                            LeftPredicate,
        Func<ITreeNode, bool>                            StatementPredicate,
        Func<ITreeNode, long?>                           OffsetProvider,
        Func<ITreeNode, IList<ICSharpArgument>>          ArgumentsProvider,
        string                                           AlignKeyGroup
    );
}
