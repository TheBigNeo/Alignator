using TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Named;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED_ALL"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.NamedArgumentsFormattingTest.TestAlignArgumentNamesAll"/>
internal class AlignArgumentNamesAll
{
  public void Run()
  {
    // Mixed list with the positional arguments first - the unnamed arguments start in the
    // value column that the named rule builds, and the named values share that column
    Spinosaurus spino = new(
        "Spino",
        6000,
        Diet.Carnivore,
        isCarnivore: true,
        dietType: Diet.Herbi,
        park: "Jurassic Park 3");

    // Interleaved positional and named arguments - the unnamed arguments still start in the
    // value column; the named values cannot chain across the unnamed lines and stay as they are
    Spinosaurus spinoAll = new(
        "Spino",
        diet: "Carnivore",
        6000,
        isCarnivore: true,
        Diet.Carnivore,
        park: "Jurassic Park 3");

    // Mixed invocation with the positional arguments first
    Caller.TransportDinosaursToIslaSorna(
        "Blue",
        42,
        spino: "T-Rex",
        baryo: "Carno");

    // An unnamed argument in the middle of a line is not moved
    Brachiosaurus brachio = new(
        "Brachio",
        diet: "Herbi", 42,
        park: "Jurassic Park");

    // A named argument on the opening-parenthesis line makes the rule stand back
    Velociraptor raptor = new("Raptor", diet: "Carnivore",
        weight: 150,
        dietType: Diet.Herbi);

    // A purely positional multi-line list stays untouched - it belongs to the comma alignment rules
    Triceratops trike = new(
        "Triceratops",
        "Herbi",
        9000);

    // A single-line mixed list stays untouched
    var moros = new Moros("Moros", diet: "Carnivore", 70, isCarnivore: true);
  }
}
