#nullable enable
using System.Collections.Generic;
using TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;
using static TBN.Alignator.Tests.Data.RecordFactory;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Mixed;

/// <summary>
///     Showcase of every Alignator alignment context in one file - one block per setting family,
///     so the effect of combining all settings can be seen at a glance. The formatting test and
///     its gold file are created once the blocks for the real test are picked from this class.
/// </summary>
/// <seealso cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_FIRST"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_FIRST"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_WITH_EXPRESSION_ASSIGNMENTS"/>
internal class AllSettingsAtOnce
{
  [TestCase("Tyrannosaurus",      "Rex")]
  [TestCase("Velociraptor",       "Raptor")]
  [TestCase("Pachycephalosaurus", "Pachy")]
  static void CallFunctions()
  {
    Caller.FeedDinosaurs("T-Rex", "Raptor", "Moros", "Triceratops");
    Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor", "Carno", "Spino");
    Caller.FeedDinosaurs("Veloci", "Baryo", "Moso", "Dilophosaurus");

    var tRex = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
    var raptor = new Velociraptor("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2 - The Lost World") {Height = 2};
    var trike = new Triceratops("Triceratops", "Herbi", 9000, false, Diet.Herbi) {Height                                        = 3};
    var moros = new Moros("Moros", "Carnivore", 70, true, Diet.Carnivore, null);

    Tyrannosaurus tRex2 = new(name: "T-Rex",
      diet: "Carnivore",
      weight: 8000,
      isCarnivore: true,
      dietType: Diet.Carnivore,
      park: "Jurassic Park");

    Dictionary<string, string?> indominusRexFacts = new()
    {
      {"Name", "Indominus Rex"},
      {"Diet", "Carnivore"},
      {"Period", "Cretaceous"},
      {"Weight", "8,000 kg"},
      {"Enclosure", "Paddock 11"},
      {"Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},
    };

    DinosaurRecord indominusRex = GetDinosaurRecord(ParkSettings.Instance)
      with
      {
        Name = "Indominus Rex",
        Diet = "Carnivore",
        PaddockNumber = 11,
        DangerLevel = DangerLevel.Extreme,
        ThermalCamouflageEnabled = true,
      };
  }
}
