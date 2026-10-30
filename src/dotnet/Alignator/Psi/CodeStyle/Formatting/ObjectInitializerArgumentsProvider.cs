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
///     Registers the alignment rules for object initializers
///     (<c>new Type() { .. }</c>) inside one collection -
///     a collection expression (<c>[ .. ]</c>) or a collection initializer (<c>new List&lt;T&gt; { .. }</c>):
///     opening brace alignment (<c>ALIGN_OBJECT_INITIALIZER_OPENING_BRACES</c>),
///     assignment alignment (<c>ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS</c>)
///     and closing brace alignment (<c>ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES</c>).
/// </summary>
[Language(typeof(CSharpLanguage))]
public class ObjectInitializerArgumentsProvider : AlignArgumentsProviderBase
{
    public ObjectInitializerArgumentsProvider(ISettingsSchema           settingsSchema,
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

        ALIGN_OBJECT_INITIALIZER_OPENING_BRACES_SpaceBeforeBrace();
        ALIGN_OBJECT_INITIALIZER_OPENING_BRACES_SpaceAfterBrace();
        ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS_BeforeEq();
        ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS_AfterComma();
        ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES_BeforeClosingBrace();
    }

    /// <summary>
    ///     Aligns the opening braces of one collection's object initializer entries by padding before the <c>{</c>,
    ///     so the braces share one column when the entries are written on separate lines.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Tyrannosaurus[] paddock =
    ///     [
    ///       new("T-Rex")       { Height = 2 },
    ///       new("Velociraptor") { Height = 1 },
    ///     ];
    ///     </code>
    /// </example>
    private void ALIGN_OBJECT_INITIALIZER_OPENING_BRACES_SpaceBeforeBrace()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES) + "_SpaceBeforeBrace")
           .Where(
                // The gap starts at the creation's closing parenthesis -
                // a sibling of the argument list, not a child of it -
                // or at its type usage when the creation is written without parentheses.
                // It ends at the object initializer, whose first child is the '{';
                // the gap's parent node is the object creation itself.
                Left().Satisfies((node, _) => node.NodeOrNull is ITokenNode closingParen
                                            && closingParen.GetTokenType() == CSharpTokenType.RPARENTH
                                          || node.NodeOrNull is ITypeUsage),
                Right().Satisfies((node, _) => node.NodeOrNull is IObjectInitializer),
                Parent().Satisfies((node, _) => node.NodeOrNull is IObjectCreationExpression)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES,
                When(OpeningBracePosition.SpaceBeforeBrace).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the entry's object creation
                        if (ctx.Parent.NodeOrNull is not IObjectCreationExpression creation)
                        {
                            return null;
                        }

                        var collectionOffset = TryGetAlignableCollectionOffset(creation);

                        if (collectionOffset is null)
                        {
                            return null;
                        }

                        // One shared key per collection, like the dictionary entry rules:
                        // every opening brace aligns to one common column
                        return new IntAlignOptionValue(
                            $"ObjInit$BeforeOpenBrace$Coll{collectionOffset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Aligns the first members of one collection's object initializer entries by padding after the <c>{</c>,
    ///     so the member starts share one column behind the untouched braces.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Tyrannosaurus[] paddock =
    ///     [
    ///       new("T-Rex") {       Height = 2 },
    ///       new("Velociraptor") { Height = 1 },
    ///     ];
    ///     </code>
    /// </example>
    private void ALIGN_OBJECT_INITIALIZER_OPENING_BRACES_SpaceAfterBrace()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES) + "_SpaceAfterBrace")
           .Where(
                Left().HasType(CSharpTokenType.LBRACE),
                Right().Satisfies((node, _) => node.NodeOrNull is IMemberInitializer),
                Parent().Satisfies((node, _) => node.NodeOrNull is IObjectInitializer)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES,
                When(OpeningBracePosition.SpaceAfterBrace).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the entry's object initializer;
                        // its parent is the object creation of the entry
                        if (ctx.Parent.NodeOrNull is not IObjectInitializer { Parent: IObjectCreationExpression creation })
                        {
                            return null;
                        }

                        var collectionOffset = TryGetAlignableCollectionOffset(creation);

                        if (collectionOffset is null)
                        {
                            return null;
                        }

                        // One shared key per collection, like the dictionary entry rules:
                        // every first member start aligns to one common column.
                        // The key is disjoint from the SpaceBeforeBrace key,
                        // so both settings combine without padding any line twice
                        return new IntAlignOptionValue(
                            $"ObjInit$AfterOpenBrace$Coll{collectionOffset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Aligns the member assignments of one collection's object initializer entries by padding before the <c>=</c>,
    ///     so the equals signs of the k-th member share one column.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Tyrannosaurus[] paddock =
    ///     [
    ///       new("T-Rex")       { Height = 2, DangerLevel = DangerLevel.Medium },
    ///       new("Velociraptor") { Mass   = 15, DangerLevel = DangerLevel.Extreme },
    ///     ];
    ///     </code>
    /// </example>
    private void ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS_BeforeEq()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS) + "_BeforeEq")
           .Where(
                Left().Satisfies((node, _) => node.NodeOrNull is ICSharpIdentifier),
                Right().Satisfies((node, _) => node.NodeOrNull is ITokenNode token
                                            && token.GetTokenType() == CSharpTokenType.EQ),
                Parent().Satisfies((node, _) => node.NodeOrNull is IMemberInitializer)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the entry's member initializer;
                        // walking up reaches the entry's object creation,
                        // every other '=' gap is rejected
                        if (ctx.Parent.NodeOrNull is not IMemberInitializer memberInitializer)
                        {
                            return null;
                        }

                        var creation = TryGetEntryCreation(memberInitializer);

                        if (creation is null)
                        {
                            return null;
                        }

                        var collectionOffset = TryGetAlignableCollectionOffset(creation);

                        if (collectionOffset is null)
                        {
                            return null;
                        }

                        // One shared key per collection, without a member index:
                        // the engine aligns the k-th '=' gap of each line as one column group
                        return new IntAlignOptionValue(
                            $"ObjInit$BeforeEq$Coll{collectionOffset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Aligns the member starts of one collection's object initializer entries by padding after the member comma,
    ///     so the k-th members of the entries share one column.
    ///     Combines with <see cref="ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS_BeforeEq"/> under the same setting -
    ///     the keys are disjoint, so a line is never padded twice.
    /// </summary>
    private void ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS_AfterComma()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS) + "_AfterComma")
           .Where(
                Left().HasType(CSharpTokenType.COMMA),
                Right().Satisfies((node, _) => node.NodeOrNull is IMemberInitializer),
                Parent().Satisfies((node, _) => node.NodeOrNull is IObjectInitializer)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the entry's object initializer;
                        // its parent is the object creation of the entry
                        if (ctx.Parent.NodeOrNull is not IObjectInitializer { Parent: IObjectCreationExpression creation })
                        {
                            return null;
                        }

                        var collectionOffset = TryGetAlignableCollectionOffset(creation);

                        if (collectionOffset is null)
                        {
                            return null;
                        }

                        // One shared key per collection, without a member index:
                        // the engine aligns the k-th member start of each line as one column group
                        return new IntAlignOptionValue(
                            $"ObjInit$AfterComma$Coll{collectionOffset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Aligns the closing braces of one collection's object initializer entries by padding before them,
    ///     so the closing braces share one column.
    ///     Controlled by a separate setting,
    ///     so opening brace, assignment and closing brace alignment can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Tyrannosaurus[] paddock =
    ///     [
    ///       new("T-Rex") { Height = 2                    },
    ///       new("Velociraptor") { Height = 1, DangerLevel = DangerLevel.Extreme },
    ///     ];
    ///     </code>
    /// </example>
    private void ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES_BeforeClosingBrace()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES) + "_BeforeClosingBrace")
           .Where(
                // The gap starts at the last member initializer -
                // or at its trailing comma, which does not move the closing brace's column
                Left().Satisfies((node, _) => node.NodeOrNull is IMemberInitializer
                                            || node.NodeOrNull is ITokenNode token
                                            && token.GetTokenType() == CSharpTokenType.COMMA),
                Right().HasType(CSharpTokenType.RBRACE),
                Parent().Satisfies((node, _) => node.NodeOrNull is IObjectInitializer)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the entry's object initializer;
                        // its parent is the object creation of the entry
                        if (ctx.Parent.NodeOrNull is not IObjectInitializer { Parent: IObjectCreationExpression creation })
                        {
                            return null;
                        }

                        var collectionOffset = TryGetAlignableCollectionOffset(creation);

                        if (collectionOffset is null)
                        {
                            return null;
                        }

                        // One shared key per collection, like the dictionary entry rules:
                        // every closing brace aligns to one common column
                        return new IntAlignOptionValue(
                            $"ObjInit$BeforeCloseBrace$Coll{collectionOffset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Returns the document start offset of the collection that owns the entry when the entry is alignable:
    ///     the creation must carry a non-empty object initializer,
    ///     it must be a direct entry of one collection -
    ///     wrapped in an expression element of a collection expression (<c>[ .. ]</c>)
    ///     or the single argument of a braceless element initializer
    ///     of a collection initializer (<c>new List&lt;T&gt; { .. }</c>) -
    ///     which scopes the alignment group,
    ///     and it must start its own line and span a single line.
    ///     Returns <c>null</c> otherwise.
    /// </summary>
    private static long? TryGetAlignableCollectionOffset(IObjectCreationExpression creation)
    {
        // Only creations with a member initializer are aligned:
        // 'new Type()' without initializer and 'new Type { 1, 2 }' collection creations stay untouched
        if (creation.Initializer is not IObjectInitializer { MemberInitializers.Count: > 0 })
        {
            return null;
        }

        // The creation must be a direct entry of one collection,
        // which scopes the alignment group:
        // entries of different collections never align with each other.
        // In a collection expression the creation is wrapped in an expression element,
        // in a collection initializer in a braceless element initializer with the creation as its single argument.
        ITreeNode container = creation.Parent switch
        {
            IExpressionElement { Parent: ICollectionExpression collection }   => collection,
            ICSharpArgument { Parent: ICollectionElementInitializer { LBrace: null, Parent: ICollectionInitializer initializer } }
                                                                              => initializer,
            _ => null,
        };

        if (container is null)
        {
            return null;
        }

        // Align only entries that start their own line or open the collection:
        // entries in the middle of a line have nothing to align with
        if (!StartsOwnLineAfterToken(creation,
                token => token.GetTokenType() == CSharpTokenType.LBRACE
                      || token.GetTokenType() == CSharpTokenType.LBRACKET))
        {
            return null;
        }

        // An entry wrapped over several lines cannot be aligned
        if (ContainsLineBreak(creation))
        {
            return null;
        }

        return container.GetDocumentStartOffset().Offset;
    }

    /// <summary>
    ///     Returns the object creation that owns the member initializer, or <c>null</c> for any other node -
    ///     the walk up rejects '=' gaps outside an entry's object initializer,
    ///     like the ones of with expression initializers.
    /// </summary>
    private static IObjectCreationExpression TryGetEntryCreation(IMemberInitializer memberInitializer)
    {
        for (ITreeNode n = memberInitializer; n != null; n = n.Parent)
        {
            if (n is IObjectCreationExpression creation)
            {
                return creation;
            }
        }

        return null;
    }
}
