using System.Collections.Generic;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.ObjectInitializer;

/// <seealso cref="AlignatorSettingsKey.ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.ObjectInitializerFormattingTest.TestCollectionInitializerContainer"/>
internal class CollectionInitializerContainer
{
  static void Paddock()
  {
    List<Tyrannosaurus> paddock = new()
    {
      new Tyrannosaurus("T-Rex") { Height = 2 },
      new Tyrannosaurus("Velociraptor") { Height = 1 },
      new Tyrannosaurus("Indominus Rex") { Height = 4 },
    };
  }
}
