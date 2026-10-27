// ReSharper disable InconsistentNaming

using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.EditorConfig;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

[EditorConfigKey("csharp")]
[SettingsKey(typeof(CodeFormattingSettingsKey), "Settings for Alignator plugin.")]
public class AlignatorSettingsKey
{
    [EditorConfigEntryAlias("alignator_align_arguments_in_constructor", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align arguments in constructor")]
    public AlignPosition INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR;

    [EditorConfigEntryAlias("alignator_align_arguments_in_function", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align argument in function")]
    public AlignPosition INT_ALIGN_ARGUMENTS_IN_FUNCTION;

    [EditorConfigEntryAlias("alignator_align_arguments_in_attribute", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align arguments in attribute")]
    public AlignPosition INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE;

    [EditorConfigEntryAlias("alignator_align_closing_paren_in_attribute", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(false, "Align closing parenthesis in attribute")]
    public bool INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE;

    [EditorConfigEntryAlias("alignator_align_argument_names", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(false, "Align argument names")]
    public bool INT_ALIGN_ARGUMENT_NAMES;

    [EditorConfigEntryAlias("alignator_align_key_values_in_collection_initializer", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align key-values in collection initializer")]
    public AlignPosition INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER;

    [EditorConfigEntryAlias("alignator_align_closing_braces_in_collection_initializer", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(false, "Align closing braces in collection initializer")]
    public bool INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER;

    [EditorConfigEntryAlias("alignator_align_assignments_in_with_expression", EditorConfigAliasType.LanguageSpecificStandard)]
    [SettingsEntry(false, "Align assignments in with expression")]
    public bool INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION;
}
