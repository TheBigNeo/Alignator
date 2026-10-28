namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Function;

/// <seealso cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.FunctionArgumentsFormattingTest.TestAlignArgumentsInFunction_ClosingParen"/>
internal class AlignArgumentsInFunction_ClosingParen
{
  static void CallFunctions()
  {
    Caller.Function1("T-Rex", "Raptor", "Moros", "Triceratops");
    Caller.ThatIsADifferentFunction("Gallimimus", "Indor", "Carno", "Spino");
    Caller.Function1("Veloci", "Baryo", "Moso", "Dilophosaurus");
  }

  static void CallFunctionsWithWrappedArguments()
  {
    Caller.Function1("Ankylo",
        "Brachio");
    Caller.Function1("Veloci",
        "Tyrannosaurus");

    // A closing parenthesis wrapped onto its own line stays untouched
    Caller.Function1("Moso",
        "Indor",
        "Spino"
    );
  }
}
