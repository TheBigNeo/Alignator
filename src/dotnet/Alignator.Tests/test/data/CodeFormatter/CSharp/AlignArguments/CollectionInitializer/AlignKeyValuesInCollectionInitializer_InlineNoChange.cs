#nullable enable
using System.Collections.Generic;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments.CollectionInitializer;

/// <seealso cref="AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_KEY_VALUES"/>
/// <seealso cref="AlignatorSettingsKey.ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES"/>
/// <seealso cref="TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments.CollectionInitializerFormattingTest.TestAlignKeyValuesInCollectionInitializer_InlineNoChange"/>
internal class AlignKeyValuesInCollectionInitializer_InlineNoChange
{
  static void InlineFacts()
  {
    Dictionary<string, string?> dinoFacts = new() {{"Name", "Indominus Rex"}, {"Diet", "Carnivore"}, {"Period", "Cretaceous"}};
  }
}
