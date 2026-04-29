using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Resources.Shell;

namespace TBN.Alignator;

[ZoneDefinition]
// [ZoneDefinitionConfigurableFeature("Title KUCHEN", "Description", IsInProductSection: false)]
public interface IAlignatorZone : IZone, IRequire<ILanguageCSharpZone>, IRequire<PsiFeaturesImplZone>, IRequire<ICodeEditingZone>;