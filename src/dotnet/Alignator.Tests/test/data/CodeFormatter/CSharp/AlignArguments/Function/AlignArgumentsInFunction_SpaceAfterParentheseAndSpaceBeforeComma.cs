namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function;

/// <seealso cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.FunctionArgumentsFormattingTest.TestAlignArgumentsInFunction_SpaceAfterParentheseAndSpaceBeforeComma"/>
internal class AlignArgumentsInFunction_SpaceAfterParentheseAndSpaceBeforeComma
{
  static void CallFunctions()
  {
    Caller.Function1("T-Rex", "Raptor", "Moros", "Triceratops");
    Caller.ThatIsADifferentFunction("Gallimimus", "Indor", "Carno", "Spino");
    Caller.Function1("Veloci", "Baryo", "Moso", "Dilophosaurus");
  }
}
