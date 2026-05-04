// using System;
// using System.Linq.Expressions;
// using JetBrains.Annotations;
// using JetBrains.Application.Settings;
// using JetBrains.Application.UI.Options;
// using JetBrains.Application.UI.Options.OptionPages;
// using JetBrains.Application.UI.Options.OptionsDialog;
// using JetBrains.IDE.UI.Extensions;
// using JetBrains.IDE.UI.Options;
// using JetBrains.Lifetimes;
// using JetBrains.ReSharper.Feature.Services.OptionPages.CodeEditing;
// using JetBrains.ReSharper.Feature.Services.OptionPages.CodeStyle;
// using JetBrains.ReSharper.Feature.Services.Resources;
// using TBN.Alignator.Psi.CodeStyle.Formatting;
// using JetBrains.ReSharper.Feature.Services.OptionPages.CodeStyle; // ← add this using
//
// namespace TBN.Alignator.Settings;
//
// [OptionsPage(PID, "Alignator", typeof(FeaturesEnvironmentOptionsThemedIcons.StringFormat), ParentId = "CodeStylePage")]
// public class AlignatorOptionsPage : BeSimpleOptionsPage
// {
//     private const string PID = "Alignator";
//
//     public AlignatorOptionsPage(
//         Lifetime                    lifetime,
//         OptionsPageContext          optionsPageContext,
//         OptionsSettingsSmartContext optionsSettingsSmartContext,
//         bool                        wrapInScrollablePanel = false)
//         : base(lifetime, optionsPageContext, optionsSettingsSmartContext, wrapInScrollablePanel)
//     {
//         AddHeader("INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR");
//         AddComboOptionFromEnum(
//             (AlignatorSettingsKey settings) => settings.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
//             type =>
//             {
//                 return type switch
//                        {
//                            AlignPosition.DoNotChange      => "DoNotChange",
//                            AlignPosition.SpaceAfterComma  => "SpaceAfterComma",
//                            AlignPosition.SpaceBeforeComma => "SpaceBeforeComma",
//                            _                              => throw new ArgumentOutOfRangeException(nameof(type), type, null)
//                        };
//             });
//
//         AddHeader("INT_ALIGN_ARGUMENTS_IN_FUNCTION");
//         AddComboOptionFromEnum(
//             (AlignatorSettingsKey settings) => settings.INT_ALIGN_ARGUMENTS_IN_FUNCTION,
//             type =>
//             {
//                 return type switch
//                        {
//                            AlignPosition.DoNotChange      => "DoNotChange",
//                            AlignPosition.SpaceAfterComma  => "SpaceAfterComma",
//                            AlignPosition.SpaceBeforeComma => "SpaceBeforeComma",
//                            _                              => throw new ArgumentOutOfRangeException(nameof(type), type, null)
//                        };
//             });
//     }
// }
//
// // public static class Accessor
// // {
// //     [NotNull] public static readonly Expression<Func<AlignatorSettingsKey, AlignPosition>> INT_ALIGN_ARGUMENTS_IN_FUNCTION    = x => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION;
// //     [NotNull] public static readonly Expression<Func<AlignatorSettingsKey, AlignPosition>> INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR = x => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR;
// // }