namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer;

/// <seealso cref="AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS_DECLARED"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.ObjectInitializerFormattingTest.TestDeclaredAssignments"/>
internal class DeclaredAssignments
{
  private Velociraptor _raptor = new Velociraptor("Raptor", "Carnivore", 150) { Height = 2 };
  private Triceratops _trike = new Triceratops("Triceratops", "Herbi", 9000) { Height = 3 };
  public Tyrannosaurus TRex { get; } = new Tyrannosaurus("T-Rex", "Carnivore", 8000) { Height = 5 };

  static void Paddock()
  {
    Velociraptor raptor = new("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park") { Height = 2 };
    Triceratops trike = new("Triceratops", "Herbi", 9000, false, Diet.Herbi, null) { Height = 3 };
    int sortIndex = 1;
    Brachiosaurus diloph = new("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null) { Description = "Spitter" };
    Spinosaurus spino = new("Spino", "Carnivore", 7000, true, Diet.Carnivore, null) { Height = 4 };
  }

  static void UntouchedAssignments()
  {
    Velociraptor raptor = new Velociraptor("Raptor");
    Triceratops trike = new Triceratops("Triceratops");
    raptor = new Velociraptor("Raptor", "Carnivore", 150) { Height = 2 };
    trike = new Triceratops("Triceratops", "Herbi", 9000) { Height = 3 };
  }

  static void UntouchedCollections()
  {
    Tyrannosaurus[] paddock =
    [
      new Tyrannosaurus("T-Rex") { Height = 2 },
      new Tyrannosaurus("Indominus Rex") { Height = 4 },
    ];
  }
}
