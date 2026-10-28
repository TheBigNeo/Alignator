using TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Attributes;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.AttributeArgumentsFormattingTest.TestAlignArgumentsInAttribute_SpaceAfterComma"/>
[TestCase("Tyrannosaurus", "Rex")]
[TestCase("Velociraptor", "Raptor")]
[TestCase("Moros", "Moros")]
[TestCase("Pachycephalosaurus", "Pachy")]
internal class AlignArgumentsInAttribute_SpaceAfterComma
{
  public void MyTest()
  {
  }
}