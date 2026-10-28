namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.ALIGN_CONSTRUCTOR_ARGUMENTS"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.CSharpQuirkyFormattingTest.TestAlignArgumentsInConstructor_SpaceBeforeParentheseAndCommaDoNotChange"/>
internal class AlignArgumentsInConstructor_SpaceBeforeParentheseAndCommaDoNotChange
{
  static void DefinedType()
  {
    var sortIndex = 1;

    var tRex = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
    var moros = new Moros("Moros", "Carnivore", 70, true, Diet.Carnivore, null);
    var raptor = new Velociraptor("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
    var trike = new Triceratops("Triceratops", "Herbi", 9000, false, Diet.Herbi, null) { Height = 3 };
    var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null);
  }

  static void TargetType()
  {
    var sortIndex = 1;

    Tyrannosaurus tRex = new("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
    Moros moros = new("Moros", "Carnivore", 70, true, Diet.Carnivore, null);
    Velociraptor raptor = new("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
    Triceratops trike = new("Triceratops", "Herbi", 9000, false, Diet.Herbi, null) { Height = 3 };
    Brachiosaurus diloph = new("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null);
  }
}
