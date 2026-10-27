using static TBN.Alignator.Tests.Data.RecordFactory;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.CSharpQuirkyFormattingTest.TestAlignAssignmentsInWithExpression_DoNotChange"/>
internal class AlignAssignmentsInWithExpression_DoNotChange
{
  static void IndominusRex()
  {
    DinosaurRecord indominusRex = GetDinosaurRecord(ParkSettings.Instance)
      with
      {
        Name = "Indominus Rex",
        Diet = "Carnivore",
        PaddockNumber = 11,
        DangerLevel = DangerLevel.Extreme,
        ThermalCamouflageEnabled = true,
      };
  }
}
