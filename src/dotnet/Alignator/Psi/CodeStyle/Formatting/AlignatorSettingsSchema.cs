#nullable enable

using JetBrains.Application.Components;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.OptionPages;
using JetBrains.ReSharper.Feature.Services.OptionPages.CodeStyle;
using JetBrains.ReSharper.Features.Altering.CodeFormatter.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;

namespace TBN.Alignator.Psi.CodeStyle.Formatting;

[FormattingSettingsPresentationComponent]
public class AlignatorSettingsSchema : OthersPageSchemaPart
{
    public AlignatorSettingsSchema(Lifetime                       lifetime,
                                   IContextBoundSettingsStoreLive smartContext,
                                   IValueEditorViewModelFactory   itemViewModelFactory,
                                   IComponentContainer            container,
                                   ISettingsToHide                settingsToHide,
                                   ICalculatedSettingsSchema      calculatedSettingsSchema
    )
        : base(lifetime, smartContext, itemViewModelFactory, container, settingsToHide, calculatedSettingsSchema)
    {
    }

    protected override string PagePartName => "Alignator align arguments";

    protected override void DescribePart(SchemaBuilder builder)
    {
        // The formatter runs over the code that is given to the setting inside ItemFor.
        // It is used to demo the setting's effect.

        builder
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
                    """
                    static void DefinedType()
                    {
                      var sortIndex = 1;

                      var tRex = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
                      var raptor = new Velociraptor("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
                      var trike = new Triceratops("Triceratops", "Herbivore", 9000, false, Diet.Herbivore) { Height = 3 };
                      var ankylo = new Brachiosaurus("Ankylo", "Herbivore", 6000, false, 250) { Description = "Armored" };
                      var brachio = new Tyrannosaurus("Brachio", "Herbivore", 30000, false) { Description = "Long neck" };
                      var galli = new Velociraptor("Gallimimus", "Omnivore", 200, true, 4444);
                      var spino = new Triceratops("Spino", "Carnivore", 7000, true, Diet.Carnivore, null);
                      var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null);
                    }

                    static void TargetType()
                    {
                      var sortIndex = 1;

                      Tyrannosaurus tRex = new("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
                      Velociraptor raptor = new("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
                      Triceratops trike = new("Triceratops", "Herbivore", 9000, false, Diet.Herbivore) { Height = 3 };
                      Brachiosaurus ankylo = new("Ankylo", "Herbivore", 6000, false, 250) { Description = "Armored" };
                      Tyrannosaurus brachio = new("Brachio", "Herbivore", 30000, false) { Description = "Long neck" };
                      Velociraptor galli = new("Gallimimus", "Omnivore", 200, true, 4444);
                      Triceratops spino = new("Spino", "Carnivore", 7000, true, Diet.Carnivore, null);
                      Brachiosaurus diloph = new("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null);
                    }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION,
                    """
                            static void CallFunctions()
                            {
                              Caller.F1("T-Rex",  "Raptor",        "Moros",         "Triceratops");
                              Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
                              Caller.F2("Gallimimus", "Indor", "Carno", "Spino");
                              Caller.F1("Veloci", "Baryo", "Moso", "Dilophosaurus");
                              Caller.F3("T-Rex", "Raptor", "Moros", "Triceratops");
                              Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
                              Caller.F4("Gallimimus", "Indor", "Carno", "Spino");
                              Caller.F1("Veloci", "Baryo", "Moso", "Dilophosaurus");
                            }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE,
                    """
                    [TestCase("Änderung", "Anderung")]
                    [TestCase("Café", "Cafe")]
                    [TestCase("Zürich", "Zurich")]
                    [TestFixture]
                    public void MyTest() { }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE,
                    """
                    [TestCase("Änderung", "Anderung")]
                    [TestCase("Café", "Cafe")]
                    [TestCase("Zürich", "Zurich")]
                    [TestFixture]
                    public void MyTest() { }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
            ;

        // Don't call Build() here, it will complain.
        // Build() is called in the parent schema's Describe() method.
    }

    private static void IntAlignFixup(IContextBoundSettingsStore boundSettingsStore)
    {
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, bool>(key => key.KEEP_USER_LINEBREAKS, true);
        boundSettingsStore.SetValue<CSharpFormatSettingsKey, bool>(key => key.WRAP_LINES,           false);
    }
}