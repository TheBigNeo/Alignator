using TBN.Alignator.Psi.CodeStyle.Formatting;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION"/>
internal class AlignArgumentsInFunction_SpaceBeforeComma
{
  static void CallFunctions()
  {
    Caller.F1("T-Rex",  "Raptor",        "Moros",         "Triceratops");
    Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
    Caller.F2("Gallimimus", "Indor", "Carno", "Spino");
    Caller.F1("Veloci", "Baryo", "Moso", "Dilophosaurus");
    Caller.F3("T-Rex", "Raptor", "Moros", "Triceratops");
    Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
    Caller.F4("Gallimimus", "Indor", "Carno", "Spino");
    Caller.F1("Veloci", "Baryo", "Moso", "Dilophosaurus");
  }
}