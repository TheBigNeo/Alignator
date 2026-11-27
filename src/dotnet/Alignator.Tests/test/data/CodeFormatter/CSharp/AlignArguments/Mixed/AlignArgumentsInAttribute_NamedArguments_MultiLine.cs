using TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Mixed;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.MixedContextsFormattingTest.TestAlignArgumentsInAttribute_NamedArguments_MultiLine"/>
internal class AlignArgumentsInAttribute_NamedArguments_MultiLine
{
  // Multi-line mixed lists stand back from comma alignment - the names rule pads the named arguments
  [TestCase("Tyrannosaurus", "Rex",
      park = "Jurassic Park",
      raptor = "Blue")]
  [TestCase("Gallimimus", "Echo",
      park = "Jurassic Park",
      raptor = "Delta")]

  // Single-line mixed lists stand back as well - no line is padded next to a named argument
  [TestCase("Pachycephalosaurus", "Pachy", park = "Jurassic Park")]
  [TestCase("Triceratops", "Blue", park = "Isla Nublar")]

  // Purely positional attributes still align their commas
  [TestCase("Velociraptor", "Rex")]
  [TestCase("Spinosaurus", "Sail")]
  [TestCase("Tyrannosaurus",
      "Rex")]

  // A fully named multi-line list is padded by the names rule alone
  [TestCase(name = "Tyrannosaurus",
      diet = "Carnivore")]
  public void MyTest()
  {
  }
}
