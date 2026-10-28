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

        ALIGN_CONSTRUCTOR_ARGUMENTS_SpaceBeforeComma();
        ALIGN_CONSTRUCTOR_ARGUMENTS_SpaceAfterComma();
        ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_SpaceBeforeParenthese();
        ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_SpaceAfterParenthese();
        ALIGN_CONSTRUCTOR_ARGUMENTS_Assignment_SpaceBeforeComma();
        ALIGN_CONSTRUCTOR_ARGUMENTS_Assignment_SpaceAfterComma();
        ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_Assignment_SpaceBeforeParenthese();
        ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_Assignment_SpaceAfterParenthese();
    }

    /// <summary>
    ///     Aligns arguments of object creations in local variable declarations
    ///     by padding before the comma, so the commas share one column per argument index.
    /// </summary>
    /// <example>
    ///     <code>
    ///     var tRex  = new Tyrannosaurus("T-Rex"    , "Carnivore");
    ///     var trike = new Triceratops("Triceratops", "Herbi");
    ///     </code>
    /// </example>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),
                SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS,
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
    ///     var trike = new Triceratops("Triceratops", "Herbi");
    ///     </code>
    /// </example>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS),
                SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: node => node is IDeclarationStatement,
                OffsetProvider: null,
                AlignKeyGroup: "Params",
                Position: AlignPosition.SpaceAfterComma
            ));

    /// <summary>
    ///     Aligns the first argument of object creations in local variable declarations by padding
    ///     before the opening parenthesis, so the parentheses - and with them the first
    ///     arguments - share one column. The remaining arguments are aligned by the
    ///     <see cref="AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS"/> comma setting -
    ///     the two settings are independent and can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     var tRex  = new Tyrannosaurus("T-Rex",       "Carnivore");
    ///     var moros = new Moros        ("Moros",       "Carnivore");
    ///     var trike = new Triceratops  ("Triceratops", "Herbi");
    ///     </code>
    /// </example>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_SpaceBeforeParenthese()
        => BuildIntAlignFirstArgumentRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST),
                SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: node => node is IDeclarationStatement,
                OffsetProvider: null,
                AlignKeyGroup: "Params",
                FirstArgument: FirstArgumentPosition.SpaceBeforeParenthese
            ));

    /// <summary>
    ///     Aligns the first argument of object creations in local variable declarations by padding
    ///     after the opening parenthesis, so the first arguments share one column. The remaining
    ///     arguments are aligned by the <see cref="AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS"/>
    ///     comma setting - the two settings are independent and can be enabled at the same time.
    /// </summary>
    /// <example>
    ///     <code>
    ///     var tRex   = new Tyrannosaurus(  "T-Rex",       "Carnivore");
    ///     var moros  = new Moros(          "Moros",        "Carnivore");
    ///     var trike  = new Triceratops(    "Triceratops", "Herbi");
    ///     </code>
    /// </example>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_SpaceAfterParenthese()
        => BuildIntAlignFirstArgumentRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST),
                SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: node => node is IDeclarationStatement,
                OffsetProvider: null,
                AlignKeyGroup: "Params",
                FirstArgument: FirstArgumentPosition.SpaceAfterParenthese
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
    ///     _moros  = new Moros        ("Moros", "Carnivore", 70  , true);
    ///     _raptor = new Velociraptor ("Raptor", "Carnivore", 150 , true);
    ///     </code>
    ///     Class-level field or property initializers:
    ///     <code>
    ///     private Tyrannosaurus _tRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000);
    ///     private Moros         _moros  = new Moros        ("Moros", "Carnivore", 70  );
    ///     private Velociraptor  _raptor = new Velociraptor ("Rex"  , "Carnivore", 150);
    ///     </code>
    /// </example>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_Assignment_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS) + "_Assignment",
                SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS,
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
    ///     _moros  = new Moros("Moros", "Carnivore", 70,   true);
    ///     _raptor = new Velociraptor("Raptor", "Carnivore", 150,  true);
    ///     </code>
    ///     Class-level field or property initializers:
    ///     <code>
    ///     private Tyrannosaurus _tRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true);
    ///     private Moros         _moros  = new Moros("Moros", "Carnivore", 70,    true);
    ///     private Velociraptor  _raptor = new Velociraptor("Rex",   "Carnivore", 150,  true);
    ///     </code>
    /// </example>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_Assignment_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS) + "_Assignment",
                SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: IsFieldOrPropertyAssignmentStatement,
                OffsetProvider: null,
                AlignKeyGroup: "ParamsAssign",
                Position: AlignPosition.SpaceAfterComma
            ));

    /// <summary>
    ///     Aligns the first argument of object creations assigned to fields or properties - in
    ///     assignment statements inside a body and in class-level field/property initializers -
    ///     by padding before the opening parenthesis, so the parentheses - and with them the
    ///     first arguments - share one column. The remaining arguments are aligned by the
    ///     <see cref="AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS"/> comma setting -
    ///     the two settings are independent and can be enabled at the same time.
    /// </summary>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_Assignment_SpaceBeforeParenthese()
        => BuildIntAlignFirstArgumentRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST) + "_Assignment",
                SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: IsFieldOrPropertyAssignmentStatement,
                OffsetProvider: null,
                AlignKeyGroup: "ParamsAssign",
                FirstArgument: FirstArgumentPosition.SpaceBeforeParenthese
            ));

    /// <summary>
    ///     Aligns the first argument of object creations assigned to fields or properties - in
    ///     assignment statements inside a body and in class-level field/property initializers -
    ///     by padding after the opening parenthesis, so the first arguments share one column.
    ///     The remaining arguments are aligned by the
    ///     <see cref="AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS"/> comma setting -
    ///     the two settings are independent and can be enabled at the same time.
    /// </summary>
    private void ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST_Assignment_SpaceAfterParenthese()
        => BuildIntAlignFirstArgumentRule(
            new ArgumentAlignVariant(
                SettingName: nameof(AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST) + "_Assignment",
                SettingsKey: x => x.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST,
                ParentPredicate: node => node is IArgumentList {Parent: IObjectCreationExpression},
                StatementPredicate: IsFieldOrPropertyAssignmentStatement,
                OffsetProvider: null,
                AlignKeyGroup: "ParamsAssign",
                FirstArgument: FirstArgumentPosition.SpaceAfterParenthese
            ));

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
