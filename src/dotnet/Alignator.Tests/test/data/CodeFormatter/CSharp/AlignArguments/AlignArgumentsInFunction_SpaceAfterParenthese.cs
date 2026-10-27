namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION"/>
internal class AlignArgumentsInFunction_SpaceAfterParenthese
{
  static void CallFunctions()
  {
    Caller.Function1("T-Rex", "Raptor", "Moros", "Triceratops");
    Caller.ThatIsADifferentFunction("Gallimimus", "Indor", "Carno", "Spino");
    Caller.Function1("Veloci", "Baryo", "Moso", "Dilophosaurus");
  }
}
