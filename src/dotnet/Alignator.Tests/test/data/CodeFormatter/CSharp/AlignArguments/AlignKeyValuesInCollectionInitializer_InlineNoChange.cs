namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_KEY_VALUES_IN_COLLECTION_INITIALIZER"/>
/// <seealso cref="AlignatorSettingsKey.INT_ALIGN_CLOSING_BRACES_IN_COLLECTION_INITIALIZER"/>
internal class AlignKeyValuesInCollectionInitializer_InlineNoChange
{
  static void InlineFacts()
  {
    Dictionary<string, string?> dinoFacts = new() {{"Name", "Indominus Rex"}, {"Diet", "Carnivore"}, {"Period", "Cretaceous"}};
  }
}
