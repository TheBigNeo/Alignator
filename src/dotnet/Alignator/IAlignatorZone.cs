using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Resources.Shell;

namespace TBN.Alignator;

[ZoneDefinition]
public interface IAlignatorZone : IZone, IRequire<ILanguageCSharpZone>, IRequire<PsiFeaturesImplZone>, IRequire<ICodeEditingZone>;
