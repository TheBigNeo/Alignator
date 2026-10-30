namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer;

/// <seealso cref="AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.ObjectInitializerFormattingTest.TestNamedCtorArguments"/>
internal class NamedCtorArguments
{
  static void Paddock()
  {
    Tyrannosaurus[] paddock =
    [
      new Tyrannosaurus(name: "T-Rex") { Height = 2, Description = "Alpha" },
      new Tyrannosaurus(name: "Velociraptor") { Height = 1, Description = "Beta" },
    ];
  }
}
