namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE"/>
/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE"/>
[TestCase("Tyrannosaurus", "Rex")]
[TestCase("Velociraptor", "Raptor")]
[TestCase("Moros", "Moros")]
[TestCase("Pachycephalosaurus", "Pachy")]
internal class AlignArgumentsInAttribute_SpaceBeforeCommaAndClosingParen
{
  public void MyTest()
  {
  }
}