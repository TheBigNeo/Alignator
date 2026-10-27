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
            // The built-in column-alignment engine switch: no Alignator rule takes effect
            // unless 'align variables in columns' is enabled. ItemFor binds to the original
            // CSharpFormatSettingsKey entry, so this is the same value as on the
            // ReSharper formatting page - one setting, shown in both places.
           .ItemFor((CSharpFormatSettingsKey x) => x.INT_ALIGN_VARIABLES,
                    """
                    static void DefinedType()
                    {
                      var tRex   = new Tyrannosaurus("Rex");
                      var raptor = new Velociraptor("Raptor");
                      var pachy  = new Pachycephalosaurus("Pachy");
                      var moros  = new Moros("Moros");
                    }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
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
                    [TestCase("Tyrannosaurus", "Rex")]
                    [TestCase("Velociraptor", "Raptor")]
                    [TestCase("Moros", "Moros")]
                    [TestCase("Pachycephalosaurus", "Pachy")]
                    [TestFixture]
                    public void MyTest(string name, string nickname) { }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE,
                    """
                    [TestCase("Tyrannosaurus", "Rex")]
                    [TestCase("Velociraptor", "Raptor")]
                    [TestCase("Moros", "Moros")]
                    [TestCase("Pachycephalosaurus", "Pachy")]
                    [TestFixture]
                    public void MyTest(string name, string nickname) { }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_ARGUMENT_NAMES,
                    """
                    static void CallFunctions()
                    {
                      Tyrannosaurus tRex = new(name: "T-Rex",
                          diet: "Carnivore",
                          weight: 8000,
                          isCarnivore: true,
                          dietType: Diet.Carnivore,
                          park: "Jurassic Park");

                      Velociraptor raptor = new(
                          name: "Raptor",
                          diet: "Carnivore",
                          weight: 150,
                          isCarnivore: true,
                          dietType: Diet.Carnivore,
                          park: "Jurassic Park 2") { Height = 2 };
                    }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER,
                    """
                    static void DinoFacts()
                    {
                      Dictionary<string, string> indominusRexFacts = new()
                      {
                        {"Name", "Indominus Rex"},
                        {"Diet", "Carnivore"},
                        {"Period", "Cretaceous"},
                        {"Weight", "8,000 kg"},
                        {"Enclosure", "Paddock 11"},
                        {"Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},
                      };
                    }
                    """,
                    parseType: PreviewParseType.Member,
                    previewFixup: IntAlignFixup)
           .ItemFor((AlignatorSettingsKey x) => x.INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER,
                    """
                    static void DinoFacts()
                    {
                      Dictionary<string, string> indominusRexFacts = new()
                      {
                        {"Name", "Indominus Rex"},
                        {"Diet", "Carnivore"},
                        {"Period", "Cretaceous"},
                        {"Weight", "8,000 kg"},
                        {"Enclosure", "Paddock 11"},
                        {"Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},
                      };
                    }
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
