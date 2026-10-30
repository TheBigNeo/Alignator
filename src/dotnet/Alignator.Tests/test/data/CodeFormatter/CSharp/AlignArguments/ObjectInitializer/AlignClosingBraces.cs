namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer;

/// <seealso cref="AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.ObjectInitializerFormattingTest.TestAlignClosingBraces"/>
internal class AlignClosingBraces
{
  static void Paddock()
  {
    Tyrannosaurus[] paddock =
    [
      new Tyrannosaurus("T-Rex") { Height = 2 },
      new Tyrannosaurus("Velociraptor") { Height = 1, Description = "Fast" },
      new Tyrannosaurus("Indominus Rex") { Height = 4, Description = "Hybrid" },
    ];
  }
}
