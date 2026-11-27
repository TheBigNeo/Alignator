namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer;

/// <seealso cref="AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_OPENING_BRACES"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.ObjectInitializerFormattingTest.TestAlignOpeningBraces_DoNotChange"/>
internal class AlignOpeningBraces_DoNotChange
{
  static void Paddock()
  {
    Tyrannosaurus[] paddock =
    [
      new Tyrannosaurus("T-Rex") { Height = 2 },
      new Tyrannosaurus("Velociraptor") { Height = 1 },
      new Tyrannosaurus("Indominus Rex") { Height = 4 },
    ];
  }
}
