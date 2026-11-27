namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer;

/// <seealso cref="AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.ObjectInitializerFormattingTest.TestAlignAssignments_WithOpeningBraces"/>
internal class AlignAssignments_WithOpeningBraces
{
  static void Paddock()
  {
    Tyrannosaurus[] paddock =
    [
      new Tyrannosaurus("T-Rex") { Height = 2, Description = "Alpha" },
      new Tyrannosaurus("Velociraptor") { Description = "Beta", Height = 1 },
    ];
  }
}
