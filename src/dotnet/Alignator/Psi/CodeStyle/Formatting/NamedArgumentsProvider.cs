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
///     Registers the alignment rule for the values of named arguments (<c>ALIGN_ARGUMENTS_NAMED</c>) -
///     the context-independent head of the ARGUMENTS family.
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
    }

    /// <summary>
    ///     Aligns values of named arguments of a single argument list by padding after the
    ///     argument name - the <c>name:</c> colon for calls and object creations, the <c>Name =</c>
    ///     assignment for attributes, the only legal named-argument syntax there - so the values
    ///     share one column when arguments are written on separate lines.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.F2(rex:                "T-Rex",
    ///               pachycephalosaurus: "Raptor",
    ///               gallimimus:         "Triceratops");
    ///     </code>
    /// </example>
    private void ALIGN_ARGUMENTS_NAMED_AfterNameColon()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED) + "_AfterNameColon")
           .Where(
                Left().Satisfies((node, _) => node.NodeOrNull is ITokenNode token
                                           && (token.GetTokenType() == CSharpTokenType.COLON || token.GetText() == "=")),
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

                        // Locate the gap's name token: the 'name:' colon or the 'Name =' assignment
                        // sign. Colons of ternary conditionals, case labels and similar nodes are
                        // rejected by the named-argument check below.
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
                            // 'Name = value' attribute form - attributes only allow the assignment
                            // syntax for named arguments. Reject ordinary assignments and
                            // assignments nested in call arguments.
                            if (!IsInsideAttributeArgument(gapParent))
                            {
                                return null;
                            }
                        }

                        // Align only arguments that start their own line or open the list:
                        // arguments in the middle of a line have nothing to align with, and
                        // a fully single-line call must stay untouched.
                        if (!StartsLineOrOpensList(nameToken))
                        {
                            return null;
                        }

                        // A value wrapped onto the next line after the argument name cannot be aligned
                        if (GapAfterNameTokenContainsLineBreak(nameToken))
                        {
                            return null;
                        }

                        // Walk up to the argument list (invocation / object creation) or the
                        // attribute and use its start offset as the alignment group, so exactly
                        // the named arguments of this one call are aligned together
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
    ///     Returns the first named-argument name token - the <c>name:</c> colon or the <c>Name =</c>
    ///     assignment sign - among the descendants of the gap's parent node.
    /// </summary>
    private static ITreeNode FindNameToken(ITreeNode node)
    {
        for (var child = node.FirstChild; child != null; child = child.NextSibling)
        {
            if (child is ITokenNode token && (IsColonToken(token) || token.GetText() == "="))
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
    ///     Returns <c>true</c> when the node belongs to a named argument of an attribute - the
    ///     <c>Name = value</c> form, the only named-argument syntax attributes allow. Ordinary
    ///     assignments and assignments nested in call arguments are rejected by the walk-up:
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
    ///     Returns <c>true</c> when the argument of the given name token starts its own line:
    ///     it is the first argument of the list (the walk-up reaches the opening parenthesis)
    ///     or it is preceded by a line break. Arguments in the middle of a line return <c>false</c>
    ///     at the separating comma.
    /// </summary>
    private static bool StartsLineOrOpensList(ITreeNode nameToken)
    {
        for (var n = PreviousNode(nameToken); n != null; n = PreviousNode(n))
        {
            switch (n)
            {
                case IWhitespaceNode whitespace:
                    if (whitespace.GetText().IndexOf('\n') >= 0)
                    {
                        return true;
                    }

                    break;

                case ITokenNode token when token.GetTokenType() == CSharpTokenType.LPARENTH:
                    return true;

                case ITokenNode token when token.GetTokenType() == CSharpTokenType.COMMA:
                    return false;
            }
        }

        return false;
    }

    /// <summary>
    ///     Returns <c>true</c> when the whitespace between the argument name token (colon or
    ///     assignment sign) and the value contains a line break - i.e. the value is wrapped onto
    ///     the next line.
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
