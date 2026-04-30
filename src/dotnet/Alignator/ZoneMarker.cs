using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using TBN.Alignator;


// Must be in the global namespace
[ZoneActivator]
public class ZoneActivator : IActivate<IAlignatorZone>;

namespace TBN.Alignator
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IAlignatorZone>;
}