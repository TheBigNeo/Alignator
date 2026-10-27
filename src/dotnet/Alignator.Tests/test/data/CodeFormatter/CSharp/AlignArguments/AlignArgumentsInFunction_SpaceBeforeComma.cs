namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ARGUMENTS_IN_FUNCTION"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.CSharpQuirkyFormattingTest.TestAlignArgumentsInFunction_SpaceBeforeComma"/>
internal class AlignArgumentsInFunction_SpaceBeforeComma
{
  static void CallFunctions()
  {
    Caller.Function1("T-Rex"                    , "Raptor"       , "Moros"        , "Triceratops");
    Caller.Function1("Ankylo"                   , "Tyrannosaurus", "Indominus rex", "Brachio");
    Caller.ThatIsADifferentFunction("Gallimimus", "Indor"        , "Carno"        , "Spino");
    Caller.Function1("Veloci"                   , "Baryo"        , "Moso"         , "Dilophosaurus");
    Caller.ThisIsFunction3("T-Rex"              , "Raptor"       , "Moros"        , "Triceratops");
    Caller.Function1("Ankylo"                   , "Tyrannosaurus", "Indominus rex", "Brachio");
    Caller.F4("Gallimimus"                      , "Indor"        , "Carno"        , "Spino");
    Caller.Function1("Veloci"                   , "Baryo"        , "Moso"         , "Dilophosaurus");
  }
}
