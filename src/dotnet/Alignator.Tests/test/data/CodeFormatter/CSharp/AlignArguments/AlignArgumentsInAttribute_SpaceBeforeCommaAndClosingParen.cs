namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_ATTRIBUTE"/>
/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE"/>
[TestCase("Änderung", "Anderung")]
[TestCase("Café", "Cafe")]
[TestCase("Zürich", "Zurich")]
internal class AlignArgumentsInAttribute_SpaceBeforeCommaAndClosingParen
{
  public void MyTest()
  {
  }
}