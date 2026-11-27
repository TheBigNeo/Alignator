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
///     Registers the alignment rule for the property assignments
///     of <c>with</c> expression initializers (<c>ALIGN_WITH_EXPRESSION_ASSIGNMENTS</c>).
/// </summary>
[Language(typeof(CSharpLanguage))]
public class WithExpressionAssignmentsProvider : AlignArgumentsProviderBase
{
    public WithExpressionAssignmentsProvider(ISettingsSchema           settingsSchema,
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

        ALIGN_WITH_EXPRESSION_ASSIGNMENTS_BeforeEq();
    }

    /// <summary>
    ///     Aligns the property assignments of a <c>with</c> expression initializer by padding before the <c>=</c>,
    ///     so the equals signs share one column when the assignments are written on separate lines.
    /// </summary>
    /// <example>
    ///     <code>
    ///     DinosaurRecord indominusRex = GetDinosaurRecord(ParkSettings.Instance)
    ///       with
    ///       {
    ///         Name                     = "Indominus Rex",
    ///         Diet                     = "Carnivore",
    ///         PaddockNumber            = 11,
    ///         DangerLevel              = DangerLevel.Extreme,
    ///         ThermalCamouflageEnabled = true,
    ///       };
    ///     </code>
    /// </example>
    private void ALIGN_WITH_EXPRESSION_ASSIGNMENTS_BeforeEq()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.ALIGN_WITH_EXPRESSION_ASSIGNMENTS) + "_BeforeEq")
           .Where(
                Left().Satisfies((node, _) => node.NodeOrNull is ICSharpIdentifier),
                Right().Satisfies((node, _) => node.NodeOrNull is ITokenNode token
                                            && token.GetTokenType() == CSharpTokenType.EQ),
                Parent().Satisfies((node, _) => node.NodeOrNull is IWithInitializerAssignment)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.ALIGN_WITH_EXPRESSION_ASSIGNMENTS,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // The gap's parent node is the with expression initializer assignment
                        if (ctx.Parent.NodeOrNull is not IWithInitializerAssignment assignment)
                        {
                            return null;
                        }

                        // Walk up to the with expression - every other '=' gap is rejected -
                        // and scope the alignment group to it:
                        // assignments of different with expressions never align with each other
                        IWithExpression withExpression = null;
                        for (ITreeNode n = assignment; n != null; n = n.Parent)
                        {
                            if (n is not IWithExpression)
                            {
                                continue;
                            }

                            withExpression = (IWithExpression)n;
                            break;
                        }

                        if (withExpression is null)
                        {
                            return null;
                        }

                        // Align only assignments that start their own line or open the initializer:
                        // assignments in the middle of a line have nothing to align with,
                        // and a fully single-line with expression - where every gap is the only one of its key -
                        // stays untouched
                        if (!StartsOwnLineAfterToken(assignment, token => token.GetTokenType() == CSharpTokenType.LBRACE))
                        {
                            return null;
                        }

                        // An assignment wrapped over several lines cannot be aligned
                        if (ContainsLineBreak(assignment))
                        {
                            return null;
                        }

                        // One shared key per with expression, without an element index:
                        // every '=' aligns to one common column
                        return new IntAlignOptionValue(
                            $"WithExpr$BeforeEq$With{withExpression.GetDocumentStartOffset().Offset}",
                            AlignatorPriority
                        );
                    }
                )
            )
           .Build();
    }
}
