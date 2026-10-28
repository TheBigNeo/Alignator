#nullable enable
using System.Collections.Generic;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.CSharpQuirkyFormattingTest.TestAlignKeyValuesInCollectionInitializer_SpaceAfterComma"/>
internal class AlignKeyValuesInCollectionInitializer_SpaceAfterComma
{
  static void IndominusRex()
  {
    Dictionary<string, string?> indominusRexFacts = new()
    {
      {"Name", "Indominus Rex"},
      {"Diet", "Carnivore"},
      {"Period", "Cretaceous"},
      {"Weight", "8,000 kg"},
      {"Enclosure", "Paddock 11"},
      {"Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},
    };
  }
}
