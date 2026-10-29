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
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.MixedContextsFormattingTest.TestAllSettingsAtOnce"/>
internal class AllSettingsAtOnce
{
  static Tyrannosaurus RRex = null!;
  static Velociraptor Raptor = null!;
  static Triceratops Trike = null!;
  static Moros Moros = null!;


  [TestCase("Tyrannosaurus",      "Rex")]
  [TestCase("Velociraptor",       "Raptor")]
  [TestCase("Pachycephalosaurus", "Pachy")]
  static void CallFunctions()
  {
    Caller.FeedDinosaurs("T-Rex", "Raptor", "Moros", "Triceratops");
    Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor", "Carno", "Spino");
    Caller.FeedDinosaurs("Veloci", "Baryo", "Moso", "Dilophosaurus");

    RRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
    Raptor = new Velociraptor("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
    Trike  = new Triceratops("Triceratops", "Herbi", 9000, false, Diet.Herbi) { Height                                       = 3 };
    Moros  = new Moros("Moros", "Carnivore", 70, true, Diet.Carnivore, null);

    Tyrannosaurus tRexLocal = new("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
    Moros morosLocal = new("Moros", "Carnivore", 70, true, Diet.Carnivore, null);
    Velociraptor raptorLocal = new("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
    Triceratops trikeLocal = new("Triceratops", "Herbi", 9000, false, Diet.Herbi, null) { Height                                  = 3 };
    Brachiosaurus dilophLocal = new("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null);

    Velociraptor blue = new(name: "Blue",
      diet: "Carnivore",
      weight: 150,
      isCarnivore: true,
      dietType: Diet.Carnivore,
      park: "Jurassic World");

    Spinosaurus spino = new(
      name: "Spino",
      diet: "Carnivore",
      weight: 6000,
      isCarnivore: true,
      dietType: Diet.Carnivore,
      park: "Jurassic Park 3");

    Dictionary<string, string?> indominusRexFacts = new()
    {
      { "Name", "Indominus Rex" },
      { "Diet", "Carnivore" },
      { "Period", "Cretaceous" },
      { "Weight", "8,000 kg" },
      { "Enclosure", "Paddock 11" },
      { "Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog" },
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
