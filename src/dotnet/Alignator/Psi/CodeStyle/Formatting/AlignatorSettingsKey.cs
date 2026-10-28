// ReSharper disable InconsistentNaming

using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.EditorConfig;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     One field per Alignator alignment setting; the field name is the key in the JetBrains
///     settings store and in the rule registration of <see cref="AlignArgumentsProvider"/>.
/// </summary>
/// <remarks>
///     <para>
///         Key naming scheme: <c>ALIGN_&lt;CONTEXT&gt;_&lt;FAMILY&gt;[_&lt;VARIANT&gt;]</c>.
///         The context (ATTRIBUTE, COLLECTION_INITIALIZER, CONSTRUCTOR, FUNCTION,
///         WITH_EXPRESSION, ...) comes first, so all keys of one context sort into one
///         alphabetical block. The family is the stem that names what is aligned -
///         ARGUMENTS, KEY_VALUES, ASSIGNMENTS - and every key that aligns the same kind of
///         node in the same context hangs on that same stem. Variants of a stem are
///         appended as suffixes: <c>_FIRST</c> (first argument at the opening parenthesis),
///         <c>_CLOSING_PAREN</c>, <c>_CLOSING_BRACES</c>. A new variant of an existing stem
///         must extend that stem - e.g. a closing-parenthesis rule for constructors becomes
///         <c>ALIGN_CONSTRUCTOR_ARGUMENTS_CLOSING_PAREN</c> - so the family stays visible in
///         both the name and the sort order. A context-independent key uses the bare family
///         stem and sorts ahead of the contexts (<c>ALIGN_ARGUMENTS_NAMED</c> heads the
///         ARGUMENTS family); if it is later split per context, the context moves to the
///         front without breaking the scheme.
///     </para>
///     <para>
///         To add a new key:
///         <list type="number">
///             <item>Declare a public field named exactly like the key, at its alphabetical
///                   position among the existing fields.</item>
///             <item>Give it a <c>[SettingsEntry]</c> with its default value and UI text.
///                   The field name becomes the settings-store key, so a later rename
///                   discards user-set values - they fall back to the entry default.</item>
///             <item>Give it a <c>[EditorConfigEntryAlias]</c>: <c>alignator_</c> plus the
///                   key name in lowercase snake_case.</item>
///             <item>Bind it in <see cref="AlignatorSettingsSchema.DescribePart"/> with an
///                   <c>ItemFor</c> call, at the same alphabetical position.</item>
///             <item>Switch on it from a rule registered in
///                   <see cref="AlignArgumentsProvider.Initialize"/>.</item>
///         </list>
///     </para>
/// </remarks>
[EditorConfigKey("csharp")]
[SettingsKey(typeof(CodeFormattingSettingsKey), "Settings for Alignator plugin.")]
public class AlignatorSettingsKey
{
    [EditorConfigEntryAlias("alignator_align_arguments_named", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(false, "Align argument names")]
    public bool ALIGN_ARGUMENTS_NAMED;

    [EditorConfigEntryAlias("alignator_align_attribute_arguments", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align arguments in attribute")]
    public AlignPosition ALIGN_ATTRIBUTE_ARGUMENTS;

    [EditorConfigEntryAlias("alignator_align_attribute_arguments_closing_paren", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(false, "Align closing parenthesis in attribute")]
    public bool ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN;

    [EditorConfigEntryAlias("alignator_align_attribute_arguments_first", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(FirstArgumentPosition.DoNotChange, "Align first argument in attribute")]
    public FirstArgumentPosition ALIGN_ATTRIBUTE_ARGUMENTS_FIRST;

    [EditorConfigEntryAlias("alignator_align_collection_initializer_closing_braces", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(false, "Align closing braces in collection initializer")]
    public bool ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES;

    [EditorConfigEntryAlias("alignator_align_collection_initializer_key_values", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align key-values in collection initializer")]
    public AlignPosition ALIGN_COLLECTION_INITIALIZER_KEY_VALUES;

    [EditorConfigEntryAlias("alignator_align_constructor_arguments", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align arguments in constructor")]
    public AlignPosition ALIGN_CONSTRUCTOR_ARGUMENTS;

    [EditorConfigEntryAlias("alignator_align_constructor_arguments_first", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(FirstArgumentPosition.DoNotChange, "Align first argument in constructor")]
    public FirstArgumentPosition ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST;

    [EditorConfigEntryAlias("alignator_align_function_arguments", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align argument in function")]
    public AlignPosition ALIGN_FUNCTION_ARGUMENTS;

    [EditorConfigEntryAlias("alignator_align_function_arguments_first", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(FirstArgumentPosition.DoNotChange, "Align first argument in function")]
    public FirstArgumentPosition ALIGN_FUNCTION_ARGUMENTS_FIRST;

    [EditorConfigEntryAlias("alignator_align_with_expression_assignments", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(false, "Align assignments in with expression")]
    public bool ALIGN_WITH_EXPRESSION_ASSIGNMENTS;
}
