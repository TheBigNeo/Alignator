using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Impl.Tree;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Format;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     Registers the alignment rules for named arguments - the context-independent head of the ARGUMENTS family:
///     <c>ALIGN_ARGUMENTS_NAMED</c> aligns the values of named arguments,
///     and <c>ALIGN_ARGUMENTS_NAMED_ALL</c> pulls the unnamed arguments of a mixed list
///     into the same value column with an indenting rule -
///     the alignment engine cannot move a line-leading argument,
///     so its line indent carries it into the column instead.
/// </summary>
[Language(typeof(CSharpLanguage))]
public class NamedArgumentsProvider : AlignArgumentsProviderBase
{
    public NamedArgumentsProvider(ISettingsSchema           settingsSchema,
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

        ALIGN_ARGUMENTS_NAMED_AfterNameColon();
        ALIGN_ARGUMENTS_NAMED_ALL_ArgumentIndent();
    }

    /// <summary>
    ///     Aligns values of named arguments of a single argument list by padding after the argument name -
    ///     the <c>name:</c> colon for calls and object creations,
    ///     the <c>Name =</c> assignment for attributes, the only legal named-argument syntax there -
    ///     so the values share one column when arguments are written on separate lines.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.TrackPack(rex:             "T-Rex",
    ///                   pachycephalosaurus: "Raptor",
    ///                   gallimimus:         "Triceratops");
    ///     </code>
    /// </example>
    private void ALIGN_ARGUMENTS_NAMED_AfterNameColon()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED) + "_AfterNameColon")
           .Where(
                Left().Satisfies((node, _) => node.NodeOrNull is ITokenNode token
                                           && (token.GetTokenType() == CSharpTokenType.COLON || token.GetTokenType() == CSharpTokenType.EQ)),
                Right().Satisfies((node, _) => node.NodeOrNull is ICSharpExpression),
                Parent().Satisfies((_,   _) => true)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_ARGUMENTS_NAMED,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        var gapParent = ctx.Parent.NodeOrNull;
                        if (gapParent is null)
                        {
                            return null;
                        }

                        // Locate the gap's name token: the 'name:' colon or the 'Name =' assignment sign.
                        // Colons of ternary conditionals, case labels and similar nodes
                        // are rejected by the named-argument check below.
                        var nameToken = FindNameToken(gapParent);
                        if (nameToken is null)
                        {
                            return null;
                        }

                        if (IsColonToken(nameToken))
                        {
                            // 'name: value' form - the gap must sit inside the named argument, after its colon
                            if (gapParent is not ICSharpArgument {ArgumentName: not null})
                            {
                                return null;
                            }
                        }
                        else
                        {
                            // 'Name = value' attribute form - attributes only allow the assignment syntax for named arguments.
                            // Reject ordinary assignments and assignments nested in call arguments.
                            if (!IsInsideAttributeArgument(gapParent))
                            {
                                return null;
                            }
                        }

                        // Align only arguments that start their own line or open the list:
                        // arguments in the middle of a line have nothing to align with,
                        // and a fully single-line call must stay untouched.
                        if (!StartsOwnLineAfterToken(nameToken, token => token.GetTokenType() == CSharpTokenType.LPARENTH))
                        {
                            return null;
                        }

                        // A value wrapped onto the next line after the argument name cannot be aligned
                        if (GapAfterNameTokenContainsLineBreak(nameToken))
                        {
                            return null;
                        }

                        // Walk up to the argument list (invocation / object creation) or the attribute
                        // and use its start offset as the alignment group,
                        // so exactly the named arguments of this one call are aligned together
                        for (var n = nameToken; n != null; n = n.Parent)
                        {
                            if (n is not (IArgumentList or IAttribute))
                            {
                                continue;
                            }

                            return new IntAlignOptionValue(
                                $"ArgName${n.GetDocumentStartOffset().Offset}",
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
    ///     Aligns the unnamed arguments of a mixed argument list into the value column of the named arguments -
    ///     the <c>ALIGN_ARGUMENTS_NAMED_ALL</c> variant of the named-arguments rule.
    ///     The alignment engine pads only gaps without line breaks,
    ///     so an argument that starts its own line cannot be moved by an alignment rule;
    ///     this indenting rule gives the argument's line
    ///     the argument indent plus the longest named-argument name prefix instead,
    ///     so the unnamed arguments start in the value column that the named rule builds.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Spinosaurus spino = new(
    ///         "Spino",
    ///         diet:        "Carnivore",
    ///         6000,
    ///         isCarnivore: true,
    ///         park:        "Jurassic Park 3");
    ///     </code>
    /// </example>
    private void ALIGN_ARGUMENTS_NAMED_ALL_ArgumentIndent()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IndentingRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED_ALL) + "_ArgumentIndent")
           .Where(Node().HasType(ElementType.C_SHARP_ARGUMENT).Satisfies((node, _) => node.NodeOrNull is ICSharpArgument {ArgumentName: null} argument
                                       && StartsOwnLineAfterToken(argument)
                                       && !ContainsLineBreak(argument)))
           .CloseNodeGetter(NextSolidSibling)
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_ARGUMENTS_NAMED_ALL,
                When(true).Calculate((node, _) =>
                    {
                        var columnOffset = 0;

                        if (node is VirtNode virtNode && virtNode.NodeOrNull is ICSharpArgument argument)
                        {
                            var offset = CalculateValueColumnOffset(argument);

                            if (offset is null)
                            {
                                return null;
                            }

                            columnOffset = offset.Value;
                        }
                        // No node: registration probes the option tree to sort the rule into the internal indenting bucket,
                        // so the indent type must come through here

                        return new ConstantOptionNode(
                            new IndentOptionValue(
                                IndentType.StartAtExternal | IndentType.EndAtExternal | IndentType.NonSticky,
                                0,
                                new Whitespace(0, columnOffset)
                            )
                        );
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Returns the offset between the argument-line indent and the value column
    ///     that the named rule builds for the argument's list:
    ///     the longest <c>name:</c> prefix among the named arguments,
    ///     plus the one mandatory space after the colon.
    ///     The named rule pads the values into that column only while its column chains
    ///     - consecutive lines with a named-argument gap each -
    ///     so this indent matches it only when every named argument of the list starts its own line;
    ///     otherwise it returns <c>null</c> and the list stays untouched.
    /// </summary>
    private static int? CalculateValueColumnOffset(ICSharpArgument argument)
    {
        if (argument.Parent is not IArgumentList argumentList)
        {
            return null;
        }

        var maxPrefix = 0;

        foreach (var namedArgument in argumentList.Arguments)
        {
            if (namedArgument.ArgumentName is null)
            {
                continue;
            }

            // A named argument on the opening-parenthesis line or in the middle of a line
            // breaks the column chain of the named rule, or shifts it -
            // this indent cannot reproduce either, so it stands back for the whole list
            if (!StartsOwnLineAfterToken(namedArgument))
            {
                return null;
            }

            var prefix = NamePrefixLength(namedArgument);
            if (prefix > maxPrefix)
            {
                maxPrefix = prefix;
            }
        }

        return maxPrefix == 0 ? null : maxPrefix + 1;
    }

    /// <summary>Returns the length of the <c>name:</c> prefix of a named argument, colon included.</summary>
    private static int NamePrefixLength(ICSharpArgument argument)
    {
        var length = 0;

        for (var child = argument.FirstChild; child != null; child = child.NextSibling)
        {
            length += child.GetTextLength();

            if (IsColonToken(child))
            {
                break;
            }
        }

        return length;
    }

    /// <summary>
    ///     Returns the first sibling behind the node that is not whitespace.
    ///     When no solid sibling exists (the node is the last child of its parent),
    ///     the node itself is returned instead of null, so the indenting rule still gets a close node.
    /// </summary>
    private static VirtNode NextSolidSibling(VirtNode node, CodeFormattingContext _)
    {
        var next = node.NextSibling;

        while (next is { IsWhitespace: true })
        {
            next = next.NextSibling;
        }

        return next == null ? node : next;
    }

    /// <summary>
    ///     Returns the first named-argument name token
    ///     - the <c>name:</c> colon or the <c>Name =</c> assignment sign -
    ///     among the descendants of the gap's parent node.
    /// </summary>
    private static ITreeNode FindNameToken(ITreeNode node)
    {
        for (var child = node.FirstChild; child != null; child = child.NextSibling)
        {
            if (child is ITokenNode token && (IsColonToken(token) || token.GetTokenType() == CSharpTokenType.EQ))
            {
                return token;
            }

            var found = FindNameToken(child);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    /// <summary>Returns <c>true</c> when the token is the colon of a named argument.</summary>
    private static bool IsColonToken(ITreeNode node)
        => node is ITokenNode token && token.GetTokenType() == CSharpTokenType.COLON;

    /// <summary>
    ///     Returns <c>true</c> when the node belongs to a named argument of an attribute
    ///     - the <c>Name = value</c> form, the only named-argument syntax attributes allow.
    ///     Ordinary assignments and assignments nested in call arguments are rejected by the walk-up:
    ///     an argument list appears above them long before an attribute does.
    /// </summary>
    private static bool IsInsideAttributeArgument(ITreeNode node)
    {
        for (var n = node; n != null; n = n.Parent)
        {
            switch (n)
            {
                case IAttribute:
                    return true;

                case IArgumentList:
                    return false;
            }
        }

        return false;
    }

    /// <summary>
    ///     Returns <c>true</c> when the whitespace between the argument name token (colon or assignment sign)
    ///     and the value contains a line break - i.e. the value is wrapped onto the next line.
    /// </summary>
    private static bool GapAfterNameTokenContainsLineBreak(ITreeNode nameToken)
    {
        for (var n = NextNode(nameToken); n != null; n = NextNode(n))
        {
            if (n is not IWhitespaceNode whitespace)
            {
                return false;
            }

            if (whitespace.GetText().IndexOf('\n') >= 0)
            {
                return true;
            }
        }

        return false;
    }
}
