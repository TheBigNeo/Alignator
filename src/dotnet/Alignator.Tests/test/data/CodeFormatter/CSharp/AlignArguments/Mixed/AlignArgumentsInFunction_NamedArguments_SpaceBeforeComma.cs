namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.Mixed;

/// <seealso cref="AlignatorSettingsKey.ALIGN_FUNCTION_ARGUMENTS"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_NAMED"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.MixedContextsFormattingTest.TestAlignArgumentsInFunction_NamedArguments_SpaceBeforeComma"/>
internal class AlignArgumentsInFunction_NamedArguments_SpaceBeforeComma
{
  public void Run()
  {
    // Comma alignment stands back for named arguments - only the names rule pads
    Caller.FeedDinosaurs(rex: "T-Rex",
        raptor: "Carnivore",
        spino: "Jurassic Park");

    Caller.FeedDinosaurs(
      rex: "T-Rex",
      raptor: "Carnivore",
      spino: "Jurassic Park");

    // A mixed list with one named argument stands back completely
    Caller.TransportDinosaursToIslaSorna("Triceratops", "Herbi", spino: "Jurassic Park");

    // Positional arguments still align across consecutive invocations
    Caller.TransportDinosaursToIslaSorna("Triceratops", "Herbi");
    Caller.TransportDinosaursToIslaSorna("Moros", "Carnivore");
  }
}
