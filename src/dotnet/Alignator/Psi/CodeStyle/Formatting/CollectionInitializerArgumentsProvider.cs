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
///     Registers the alignment rules for dictionary-style entries of collection initializers
///     (<c>ALIGN_COLLECTION_INITIALIZER_*</c>): key-value alignment and closing brace alignment.
/// </summary>
[Language(typeof(CSharpLanguage))]
public class CollectionInitializerArgumentsProvider : AlignArgumentsProviderBase
{
    public CollectionInitializerArgumentsProvider(ISettingsSchema           settingsSchema,
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

        ALIGN_COLLECTION_INITIALIZER_KEY_VALUES_SpaceBeforeComma();
        ALIGN_COLLECTION_INITIALIZER_KEY_VALUES_SpaceAfterComma();
        ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES_BeforeClosingBrace();
    }

    /// <summary>
    ///     Aligns keys of dictionary-style collection initializer entries by padding
    ///     before the comma, so the commas share one column.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Dictionary<string, string?> indominusRexFacts = new()
    ///     {
    ///         {"Name"     , "Indominus Rex"},
    ///         {"Enclosure", "Paddock 11"},
    ///     };
    ///     </code>
    /// </example>
    private void ALIGN_COLLECTION_INITIALIZER_KEY_VALUES_SpaceBeforeComma()
        => BuildIntAlignKeyValuesInCollectionInitializerRule(AlignPosition.SpaceBeforeComma);

    /// <summary>
    ///     Aligns values of dictionary-style collection initializer entries by padding
    ///     after the comma, so the values share one column.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Dictionary<string, string?> indominusRexFacts = new()
    ///     {
    ///         {"Name",      "Indominus Rex"},
    ///         {"Enclosure", "Paddock 11"},
    ///     };
    ///     </code>
    /// </example>
    private void ALIGN_COLLECTION_INITIALIZER_KEY_VALUES_SpaceAfterComma()
        => BuildIntAlignKeyValuesInCollectionInitializerRule(AlignPosition.SpaceAfterComma);

    /// <summary>
    ///     Aligns closing braces of dictionary-style collection initializer entries by padding
    ///     before them, so they share one column. Controlled by a separate setting, so key-value
    ///     alignment and closing brace alignment can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Dictionary<string, string?> indominusRexFacts = new()
    ///     {
    ///         {"Name",      "Indominus Rex"                              },
    ///         {"Genome",    "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},
    ///     };
    ///     </code>
    /// </example>
    private void ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES_BeforeClosingBrace()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES) + "_BeforeClosingBrace")
           .Where(
                Left().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument),
                Right().HasType(CSharpTokenType.RBRACE),
                Parent().Satisfies((node, _) => node.NodeOrNull is ICollectionElementInitializer)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the collection initializer entry
                        if (ctx.Parent.NodeOrNull is not ICollectionElementInitializer entry)
                        {
                            return null;
                        }

                        // Dictionary-style entries carry a key and a value argument; simple {expr}
                        // entries have a single argument and must not be padded
                        var argumentCount = 0;
                        for (var child = entry.FirstChild; child != null; child = child.NextSibling)
                        {
                            if (child is ICSharpArgument)
                            {
                                argumentCount++;
                            }
                        }

                        if (argumentCount < 2)
                        {
                            return null;
                        }

                        // Only entries of a collection initializer of an object creation are aligned,
                        // and the initializer scopes the alignment group: entries of different
                        // initializers never align with each other
                        if (entry.Parent is not ICollectionInitializer {Parent: IObjectCreationExpression} collectionInitializer)
                        {
                            return null;
                        }

                        // Align only entries that start their own line or open the initializer:
                        // entries in the middle of a line have nothing to align with
                        if (!EntryStartsOwnLine(entry))
                        {
                            return null;
                        }

                        // An entry wrapped over several lines cannot be aligned
                        if (ContainsLineBreak(entry))
                        {
                            return null;
                        }

                        // One shared key per collection initializer, like the attribute closing
                        // parenthesis rule: every closing brace aligns to one common column
                        return new IntAlignOptionValue(
                            $"CollInitClose$BeforeCloseBrace$Coll{collectionInitializer.GetDocumentStartOffset().Offset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }

    /// <summary>
    ///     Builds the shared <see cref="IntAlignRule"/> for one key-value alignment position of
    ///     dictionary-style collection initializer entries: it matches the whitespace gap between
    ///     an entry's key argument and its comma (or between the comma and the value argument) and
    ///     aligns all gaps that share the variant's alignment key (comma mode, enclosing
    ///     collection initializer offset).
    /// </summary>
    private void BuildIntAlignKeyValuesInCollectionInitializerRule(AlignPosition position)
    {
        var isSpaceBefore = position == AlignPosition.SpaceBeforeComma;
        var nameSuffix    = isSpaceBefore ? "_SpaceBefore" : "_SpaceAfter";
        var commaKeyPart  = isSpaceBefore ? "ArgComma" : "ArgAfterComma";

        var leftCondition = isSpaceBefore
                                ? Left().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument)
                                : Left().HasType(CSharpTokenType.COMMA);

        var rightCondition = isSpaceBefore
                                 ? Right().HasType(CSharpTokenType.COMMA)
                                 : Right().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument);

        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES) + nameSuffix)
           .Where(
                leftCondition,
                rightCondition,
                Parent().Satisfies((node, _) => node.NodeOrNull is ICollectionElementInitializer)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES,
                When(position).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the collection initializer entry
                        if (ctx.Parent.NodeOrNull is not ICollectionElementInitializer entry)
                        {
                            return null;
                        }

                        // Dictionary-style entries carry a key and a value argument; simple {expr}
                        // entries have a single argument and no inner comma, so they are skipped
                        var argumentCount = 0;
                        for (var child = entry.FirstChild; child != null; child = child.NextSibling)
                        {
                            if (child is ICSharpArgument)
                            {
                                argumentCount++;
                            }
                        }

                        if (argumentCount < 2)
                        {
                            return null;
                        }

                        // Only entries of a collection initializer of an object creation are aligned,
                        // and the initializer scopes the alignment group: entries of different
                        // initializers never align with each other
                        if (entry.Parent is not ICollectionInitializer {Parent: IObjectCreationExpression} collectionInitializer)
                        {
                            return null;
                        }

                        // Align only entries that start their own line or open the initializer:
                        // entries in the middle of a line have nothing to align with
                        if (!EntryStartsOwnLine(entry))
                        {
                            return null;
                        }

                        // An entry wrapped over several lines cannot be aligned
                        if (ContainsLineBreak(entry))
                        {
                            return null;
                        }

                        // One shared key per collection initializer - no element index: the engine
                        // aligns the k-th same-key gap of each line as one column group
                        return new IntAlignOptionValue(
                            $"CollInit${commaKeyPart}$Coll{collectionInitializer.GetDocumentStartOffset().Offset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }
}
