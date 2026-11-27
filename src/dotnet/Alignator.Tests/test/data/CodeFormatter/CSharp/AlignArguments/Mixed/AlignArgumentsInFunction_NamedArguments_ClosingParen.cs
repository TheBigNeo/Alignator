namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Mixed;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.MixedContextsFormattingTest.TestAlignArgumentsInFunction_NamedArguments_ClosingParen"/>
internal class AlignArgumentsInFunction_NamedArguments_ClosingParen
{
  static void CallFunctionsWithNamedArguments()
  {
    // The named-argument rule pads between the argument names and the values, the
    // closing-parenthesis rule pads before the closing parenthesis - the two combine.
    // The closing parentheses of the two calls sit on consecutive lines, so they
    // share one column.
    Caller.TrackPack(rex: "T-Rex",
        pachycephalosaurus: "Raptor",
        gallimimus: "Moros");
    Caller.TrackPack("Veloci", "Baryo");
  }
}
