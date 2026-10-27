using static TBN.Alignator.Tests.Data.RecordFactory;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.ALIGN_ASSIGNMENTS_IN_WITH_EXPRESSION"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.CSharpQuirkyFormattingTest.TestAlignAssignmentsInWithExpression_SingleLineUntouched"/>
internal class AlignAssignmentsInWithExpression_SingleLineUntouched
{
  static void IndominusRex()
  {
    DinosaurRecord echo = GetDinosaurRecord("Echo") with { Name = "Echo", Speed = 60 };
  }
}
