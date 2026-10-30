using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Resources.Shell;

namespace TBN.Alignator;

/// <summary>
///     Declares the Alignator feature zone:
///     everything the plugin registers into the ReSharper/Rider environment requires it,
///     so the components load only where C# code editing is available.
/// </summary>
[ZoneDefinition]
public interface IAlignatorZone : IZone, IRequire<ILanguageCSharpZone>, IRequire<PsiFeaturesImplZone>, IRequire<ICodeEditingZone>;
