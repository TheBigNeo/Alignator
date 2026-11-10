using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     Registers the alignment rules for arguments of object creations (<c>ALIGN_CONSTRUCTOR_ARGUMENTS*</c>),
///     in local variable declarations and in assignments to fields or properties.
/// </summary>
[Language(typeof(CSharpLanguage))]
public class ConstructorArgumentsProvider : AlignArgumentsProviderBase
{
    public ConstructorArgumentsProvider(ISettingsSchema           settingsSchema,
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

        ALIGN_CONSTRUCTOR_ARGUMENTS_CommaPositions();
        ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_ParenPositions();
        ALIGN_CONSTRUCTOR_ARGUMENTS_Assignment_CommaPositions();
        ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_Assignment_ParenPositions();
    }

    /// <summary>
    ///     Aligns arguments of object creations in local variable declarations around the commas:
    ///     padding before the comma makes the commas share one column per argument index,
    ///     padding after it makes the arguments following them share one column per argument index.
    /// </summary>
    /// <example>
    ///     <code>
    ///     var tRex  = new Tyrannosaurus("T-Rex"      , "Carnivore");
    ///     var trike = new Triceratops("Triceratops", "Herbi");
    ///     </code>
    ///     <code>
    ///     var tRex  = new Tyrannosaurus("T-Rex",       "Carnivore");
    ///     var trike = new Triceratops("Triceratops", "Herbi");
    ///     </code>
    /// </example>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_CommaPositions()
    {
        foreach (var position in new[] {AlignPosition.SpaceBeforeComma, AlignPosition.SpaceAfterComma})
        {
            BuildIntAlignArgumentsRule(
                new ArgumentAlignVariant(
                    SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),
                    SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS,
                    ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                    StatementPredicate: node => node is IDeclarationStatement,
                    OffsetProvider: null,
                    AlignKeyGroup: "Params",
                    Position: position
                ));
        }
    }

    /// <summary>
    ///     Aligns the first argument of object creations in local variable declarations at the opening parenthesis:
    ///     padding before the parenthesis makes the parentheses - and with them the first arguments - share one column,
    ///     padding after it makes the first arguments share one column behind the untouched parentheses.
    ///     The remaining arguments are aligned by the <see cref="AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS"/>
    ///     comma setting - the two settings are independent and can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     var tRex  = new Tyrannosaurus("T-Rex",       "Carnivore");
    ///     var moros = new Moros        ("Moros",       "Carnivore");
    ///     var trike = new Triceratops  ("Triceratops", "Herbi");
    ///     </code>
    ///     <code>
    ///     var tRex  = new Tyrannosaurus(  "T-Rex",       "Carnivore");
    ///     var moros = new Moros(          "Moros",       "Carnivore");
    ///     var trike = new Triceratops(    "Triceratops", "Herbi");
    ///     </code>
    /// </example>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_ParenPositions()
    {
        foreach (var position in new[] {FirstArgumentPosition.SpaceBeforeParenthese, FirstArgumentPosition.SpaceAfterParenthese})
        {
            BuildIntAlignFirstArgumentRule(
                new ArgumentAlignVariant(
                    SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST),
                    SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST,
                    ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                    StatementPredicate: node => node is IDeclarationStatement,
                    OffsetProvider: null,
                    AlignKeyGroup: "Params",
                    FirstArgument: position
                ));
        }
    }

    /// <summary>
    ///     Aligns arguments of object creations assigned to fields or properties
    ///     - in assignment statements inside a body and in class-level field/property initializers -
    ///     around the commas:
    ///     padding before the comma makes the commas share one column per argument index,
    ///     padding after it makes the arguments following them share one column per argument index.
    /// </summary>
    /// <example>
    ///     Assignment statements inside a body:
    ///     <code>
    ///     _tRex   = new Tyrannosaurus("T-Rex" , "Carnivore", 8000, true);
    ///     _moros  = new Moros        ("Moros" , "Carnivore", 70  , true);
    ///     _raptor = new Velociraptor ("Raptor", "Carnivore", 150 , true);
    ///     </code>
    ///     Class-level field or property initializers:
    ///     <code>
    ///     private Tyrannosaurus _tRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000);
    ///     private Moros         _moros  = new Moros        ("Moros", "Carnivore", 70  );
    ///     private Velociraptor  _raptor = new Velociraptor ("Rex"  , "Carnivore", 150);
    ///     </code>
    /// </example>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_Assignment_CommaPositions()
    {
        foreach (var position in new[] {AlignPosition.SpaceBeforeComma, AlignPosition.SpaceAfterComma})
        {
            BuildIntAlignArgumentsRule(
                new ArgumentAlignVariant(
                    SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS) + "_Assignment",
                    SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS,
                    ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                    StatementPredicate: IsFieldOrPropertyAssignmentStatement,
                    OffsetProvider: null,
                    AlignKeyGroup: "ParamsAssign",
                    Position: position
                ));
        }
    }

    /// <summary>
    ///     Aligns the first argument of object creations assigned to fields or properties
    ///     - in assignment statements inside a body and in class-level field/property initializers -
    ///     at the opening parenthesis:
    ///     padding before the parenthesis makes the parentheses - and with them the first arguments - share one column,
    ///     padding after it makes the first arguments share one column behind the untouched parentheses.
    ///     The remaining arguments are aligned by the <see cref="AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS"/>
    ///     comma setting - the two settings are independent and can be enabled at the same time.
    /// </summary>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_Assignment_ParenPositions()
    {
        foreach (var position in new[] {FirstArgumentPosition.SpaceBeforeParenthese, FirstArgumentPosition.SpaceAfterParenthese})
        {
            BuildIntAlignFirstArgumentRule(
                new ArgumentAlignVariant(
                    SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST) + "_Assignment",
                    SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST,
                    ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                    StatementPredicate: IsFieldOrPropertyAssignmentStatement,
                    OffsetProvider: null,
                    AlignKeyGroup: "ParamsAssign",
                    FirstArgument: position
                ));
        }
    }

    /// <summary>
    ///     Returns <c>true</c> for the nodes that carry an object creation assigned to a field or property:
    ///     assignment expression statements and class-level field or property declarations (initializers).
    /// </summary>
    private static bool IsFieldOrPropertyAssignmentStatement(ITreeNode node)
        => node is IExpressionStatement {Expression: IAssignmentExpression}
                or IFieldDeclaration
                or IPropertyDeclaration;
}
