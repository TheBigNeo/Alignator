// ReSharper disable InconsistentNaming

using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.EditorConfig;
using JetBrains.ReSharper.Psi.Format;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

// [SettingsKey(typeof(CSharpCodeStyleSettingsKey), "Settings for Alignator plugin.")]
[SettingsKey(typeof(CodeFormattingSettingsKey),  "Settings for Alignator plugin.")]
public class AlignatorSettingsKey
{
    [EditorConfigEntryAlias("int_align", EditorConfigAliasType.Generalized)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align arguments in constructor")]
    public AlignPosition INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR;

    [EditorConfigEntryAlias("int_align", EditorConfigAliasType.Generalized)]
    [SettingsEntry(AlignPosition.DoNotChange, "Align argument in function")]
    public AlignPosition INT_ALIGN_ARGUMENTS_IN_FUNCTION;
}