using static TBN.Alignator.Tests.Data.RecordFactory;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.WithExpression;

/// <seealso cref="AlignatorSettingsKey.ALIGN_WITH_EXPRESSION_ASSIGNMENTS"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.WithExpressionFormattingTest.TestAlignAssignmentsInWithExpression_SingleLineUntouched"/>
internal class AlignAssignmentsInWithExpression_SingleLineUntouched
{
  static void IndominusRex()
  {
    DinosaurRecord echo = GetDinosaurRecord("Echo") with { Name = "Echo", Speed = 60 };
  }
}
