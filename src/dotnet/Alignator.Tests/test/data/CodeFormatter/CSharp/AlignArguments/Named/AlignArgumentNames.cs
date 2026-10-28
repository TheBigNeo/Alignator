using TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Named;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.NamedArgumentsFormattingTest.TestAlignArgumentNames"/>
internal class AlignArgumentNames
{
  public void Run()
  {
    // Multi-line invocation with named arguments, one argument per line
    Caller.Function1(rex: "T-Rex",
        raptor: "Raptor",
        spino: "Triceratops");

    // Multi-line invocation with named arguments of very different name lengths
    Caller.F2(rex: "T-Rex",
        pachycephalosaurus: "Raptor",
        gallimimus: "Triceratops");

    // Multi-line object creation with named arguments, first argument on the same line
    Tyrannosaurus tRex = new(name: "T-Rex",
        diet: "Carnivore",
        weight: 8000,
        isCarnivore: true,
        dietType: Diet.Carnivore,
        park: "Jurassic Park");

    // Mixed positional and named arguments
    Caller.ThatIsADifferentFunction(rex: "Blue",
        "T-Rex",
        spino: "Raptor");

    // Multi-line object creation with named arguments, first argument on its own line
    Velociraptor raptor = new(
        name: "Raptor",
        diet: "Carnivore",
        weight: 150,
        isCarnivore: true,
        dietType: Diet.Carnivore,
        park: "Jurassic Park 2 - The Lost World") { Height = 2 };

    // Single-line call with named arguments stays untouched
    Caller.ThisIsFunction3(rex: "T-Rex", raptor: "Raptor", spino: "Triceratops");
  }

  // Multi-line attribute with named arguments
  [TestCase(rex = "Tyrannosaurus",
      raptor = "Rex")]
  public void MyTest()
  {
  }
}
