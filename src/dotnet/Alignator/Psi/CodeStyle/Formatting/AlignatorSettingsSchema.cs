using JetBrains.Annotations;
using JetBrains.Application.Components;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.OptionPages;
using JetBrains.ReSharper.Feature.Services.OptionPages.CodeStyle;
using JetBrains.ReSharper.Features.Altering.CodeFormatter.CSharp;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

[FormattingSettingsPresentationComponent]
public class AlignatorSettingsSchema : OthersPageSchemaPart
{
    public AlignatorSettingsSchema(Lifetime                                 lifetime,
                                   [NotNull] IContextBoundSettingsStoreLive smartContext,
                                   [NotNull] IValueEditorViewModelFactory   itemViewModelFactory,
                                   [NotNull] IComponentContainer            container,
                                   [NotNull] ISettingsToHide                settingsToHide,
                                   [NotNull] ICalculatedSettingsSchema      calculatedSettingsSchema
    )
        : base(lifetime, smartContext, itemViewModelFactory, container, settingsToHide, calculatedSettingsSchema)
    {
    }

    protected override string PagePartName => "Quirky code style KUCHEN";

    protected override void DescribePart(SchemaBuilder builder)
    {
        // The formatter runs over the code that is given to the setting inside ItemFor.
        // It is used to demo the setting's effect.

        builder
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
                    "public static void Main(){var ShortName = new A(\"x\"); var LongerName = new LongerType(\"x\");}")
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION,
                    "public static void Main(){var ShortName = new A(\"x\"); var LongerName = new LongerType(\"x\");}")
            ;

        // Don't call Build() here, it will complain.
        // Build() is called in the parent schema's Describe() method.
    }
}