using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionPages;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.Resources;

namespace ReSharperPlugin.Alignator.Settings
{
    [OptionsPage(Pid, "Alignator", typeof(FeaturesEnvironmentOptionsThemedIcons.StringFormat), ParentId = EnvironmentPage.Pid)]
    public class AlignatorOptionsPage : BeSimpleOptionsPage
    {
        private const string Pid = "Alignator";

        public AlignatorOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext,
            bool wrapInScrollablePanel = false)
            : base(lifetime, optionsPageContext, optionsSettingsSmartContext, wrapInScrollablePanel)
        {
            AddHeader("Log properties naming style");
            AddHeader("Ignored properties naming");
            AddCommentText("You may specify a regular expression here and if a property matches this expression, then it will be skipped during naming analysis.");
            var ignoredRegex = OptionsSettingsSmartContext
                .GetValueProperty(lifetime, StructuredLoggingSettingsAccessor.IgnoredPropertiesRegex);
            AddControl(ignoredRegex.GetBeTextBox(lifetime));
        }
    }

    public static class StructuredLoggingSettingsAccessor
    {
        [NotNull] public static readonly Expression<Func<AlignatorSettings, string>> IgnoredPropertiesRegex = x => x.IgnoredPropertiesRegex;
    }
}