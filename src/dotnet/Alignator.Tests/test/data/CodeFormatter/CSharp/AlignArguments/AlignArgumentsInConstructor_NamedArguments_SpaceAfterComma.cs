namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_IN_CONSTRUCTOR"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENT_NAMES"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.CSharpQuirkyFormattingTest.TestAlignArgumentsInConstructor_NamedArguments_SpaceAfterComma"/>
internal class AlignArgumentsInConstructor_NamedArguments_SpaceAfterComma
{
  public void Run()
  {
    // Comma alignment stands back for named arguments - only the names rule pads
    Tyrannosaurus tRex = new(name: "T-Rex",
        diet: "Carnivore",
        weight: 8000,
        isCarnivore: true,
        dietType: Diet.Carnivore,
        park: "Jurassic Park");

    Moros moros = new(name: "Moros",
        diet: "Carnivore",
        weight: 70);

    Velociraptor raptor = new(
        name: "Raptor",
        diet: "Carnivore",
        weight: 150,
        isCarnivore: true,
        dietType: Diet.Carnivore,
        park: "Jurassic Park 2 - The Lost World") { Height = 2 };

    // A mixed list with one named argument stands back completely
    var spino = new Spinosaurus("Spino", "Carnivore", park: "Jurassic Park");

    // Positional arguments still align across consecutive object creations
    var trike = new Triceratops("Triceratops", "Herbi");
    var moros2 = new Moros("Moros", "Carnivore");
  }
}
