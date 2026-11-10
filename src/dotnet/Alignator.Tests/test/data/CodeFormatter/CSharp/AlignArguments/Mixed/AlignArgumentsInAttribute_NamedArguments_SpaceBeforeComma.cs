using TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Mixed;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.MixedContextsFormattingTest.TestAlignArgumentsInAttribute_NamedArguments_SpaceBeforeComma"/>
internal class AlignArgumentsInAttribute_NamedArguments_SpaceBeforeComma
{
  // Comma alignment stands back for named attribute arguments - only the names rule pads
  [TestCase(name = "Tyrannosaurus",
      diet = "Carnivore",
      park = "Jurassic Park")]

  // A mixed list with one named argument stands back completely
  [TestCase("Tyrannosaurus", "Rex", park = "Jurassic Park")]

  // Positional attribute arguments still align within the same attribute list
  [TestCase("Tyrannosaurus", "Rex")]
  [TestCase("Moros", "Moros")]
  public void MyTest()
  {
  }
}
