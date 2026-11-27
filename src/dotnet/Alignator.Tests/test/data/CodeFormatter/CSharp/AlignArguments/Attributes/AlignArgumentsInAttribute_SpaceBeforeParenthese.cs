using TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Attributes;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ATTRIBUTE_ARGUMENTS"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.AttributeArgumentsFormattingTest.TestAlignArgumentsInAttribute_SpaceBeforeParenthese"/>
[TyrannosaurusTestCase("Tyrannosaurus", "Rex")]
[RaptorTest("Velociraptor", "Raptor")]
[Dino("Moros", "Moros")]
[PachycephalosaurusTestCase("Pachycephalosaurus", "Pachy")]
internal class AlignArgumentsInAttribute_SpaceBeforeParenthese
{
  public void MyTest()
  {
  }
}
