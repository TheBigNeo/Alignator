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
        INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceBeforeComma();
        INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceAfterComma();
        INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE_SpaceBeforeClosingParen();
        INT_ALIGN_ARGUMENT_NAMES_AfterNameColon();
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
                OffsetProvider: null,
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
                OffsetProvider: null,
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
                OffsetProvider: null,
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
                OffsetProvider: null,
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
                OffsetProvider: null,
                AlignKeyGroup: "Func",
                Position: AlignPosition.SpaceAfterComma
            ));

    /// <summary>
    ///     Aligns arguments of attributes by padding before the comma,
    ///     so the commas share one column per argument index. Attribute arguments
    ///     are direct children of the <see cref="IAttribute"/> node - there is no argument list node.
    /// </summary>
    /// <example>
    ///     <code>
    ///     [TestCase("Tyrannosaurus"     , "Rex")]
    ///     [TestCase("Moros"             , "Moros")]
    ///     [TestCase("Pachycephalosaurus", "Pachy")]
    ///     </code>
    /// </example>
    private void INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE),
                SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE,
                ParentPredicate: node => node is IAttribute,
                StatementPredicate: node => node is IAttributeSectionList,
                OffsetProvider: AttributeSectionListOffset,
                AlignKeyGroup: "Attr",
                Position: AlignPosition.SpaceBeforeComma
            ));

    /// <summary>
    ///     Aligns arguments of attributes by padding after the comma,
    ///     so the arguments following them share one column per argument index. Attribute arguments
    ///     are direct children of the <see cref="IAttribute"/> node - there is no argument list node.
    /// </summary>
    /// <example>
    ///     <code>
    ///     [TestCase("Tyrannosaurus",      "Rex")]
    ///     [TestCase("Moros",              "Moros")]
    ///     [TestCase("Pachycephalosaurus", "Pachy")]
    ///     </code>
    /// </example>
    private void INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE),
                SettingsKey: x => x.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE,
                ParentPredicate: node => node is IAttribute,
                StatementPredicate: node => node is IAttributeSectionList,
                OffsetProvider: AttributeSectionListOffset,
                AlignKeyGroup: "Attr",
                Position: AlignPosition.SpaceAfterComma
            ));

    /// <summary>
    ///     Aligns closing parentheses of attributes by padding before them,
    ///     so they share one column. Controlled by a separate setting, so comma alignment
    ///     and closing parenthesis alignment can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     [TestCase("Tyrannosaurus"     , "Rex"   )]
    ///     [TestCase("Moros"             , "Moros" )]
    ///     [TestCase("Pachycephalosaurus", "Pachy")]
    ///     </code>
    /// </example>
    private void INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE_SpaceBeforeClosingParen()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE) + "_SpaceBeforeClosingParen")
           .Where(
                Left().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument),
                Right().HasType(CSharpTokenType.RPARENTH),
                Parent().Satisfies((node, _) => node.NodeOrNull is IAttribute)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // Walk up to the attribute section list (one per attributed declaration)
                        // and use its start offset as the alignment group
                        for (var n = ctx.Parent.NodeOrNull; n != null; n = n.Parent)
                        {
                            if (n is not IAttributeSectionList)
                            {
                                continue;
                            }

                            return new IntAlignOptionValue(
                                $"AttrClose$ArgBeforeCloseParen$Block{AttributeSectionListOffset(n)}",
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
    ///     Aligns values of named arguments of a single argument list by padding after the
    ///     argument name - the <c>name:</c> colon for calls and object creations, the <c>Name =</c>
    ///     assignment for attributes, the only legal named-argument syntax there - so the values
    ///     share one column when arguments are written on separate lines.
    /// </summary>
    /// <example>
    ///     <code>
    ///     Caller.F1(o1:   "T-Rex",
    ///               o2:   "Raptor",
    ///               o999: "Triceratops");
    ///     </code>
    /// </example>
    private void INT_ALIGN_ARGUMENT_NAMES_AfterNameColon()
    {
        DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()
           .Name(nameof(AlignatorSettingsKey.INT_ALIGN_ARGUMENT_NAMES) + "_AfterNameColon")
           .Where(
                Left().Satisfies((node, _) => node.NodeOrNull is ITokenNode token
                                              && (token.GetTokenType() == CSharpTokenType.COLON || token.GetText() == "=")),
                Right().Satisfies((node, _) => node.NodeOrNull is ICSharpExpression),
                Parent().Satisfies((_, _) => true)
            )
           .SwitchOnExternalKey(
                (AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENT_NAMES,
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

    /// <summary>Returns the node that ends right before the given node in document order, or <c>null</c>.</summary>
    private static ITreeNode PreviousNode(ITreeNode node)
    {
        if (node.PrevSibling != null)
        {
            return node.PrevSibling;
        }

        var parent = node.Parent;
        return parent is null ? null : PreviousNode(parent);
    }

    /// <summary>Returns the node that starts right after the given node in document order, or <c>null</c>.</summary>
    private static ITreeNode NextNode(ITreeNode node)
    {
        if (node.NextSibling != null)
        {
            return node.NextSibling;
        }

        var parent = node.Parent;
        return parent is null ? null : NextNode(parent);
    }

    /// <summary>
    ///     Returns the document start offset of the attribute section list, scoping attribute
    ///     alignment groups to the attributes of a single declaration.
    /// </summary>
    private static long AttributeSectionListOffset(ITreeNode node)
        => node.GetDocumentStartOffset().Offset;

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

                        // Arguments are direct children of the parent node: of an argument list
                        // for invocations and object creations, and of the attribute itself for attributes
                        System.Collections.Generic.IList<ICSharpArgument> arguments = null;

                        switch (ctx.Parent.NodeOrNull)
                        {
                            case IArgumentList argumentList:
                                arguments = argumentList.Arguments;
                                break;

                            case IAttribute attribute:
                                arguments = attribute.Arguments;
                                break;
                        }

                        if (arguments is null)
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

                        // Walk up to find the enclosing statement and use its block offset as the alignment group
                        for (var n = ctx.Parent.NodeOrNull; n != null; n = n.Parent)
                        {
                            if (!variant.StatementPredicate(n))
                            {
                                continue;
                            }

                            var blockOffset = variant.OffsetProvider != null
                                                  ? variant.OffsetProvider(n)
                                                  : n.GetContainingBlockOffset();

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
        System.Func<ITreeNode, long>                                                  OffsetProvider,
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
