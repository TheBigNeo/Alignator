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
}
