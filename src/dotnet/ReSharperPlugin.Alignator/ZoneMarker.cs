using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.ReSharper.Psi.CSharp;
using ReSharperPlugin.Alignator;


// Must be in the global namespace
[ZoneActivator]
public class ZoneActivator : IActivate<IAlignatorZone>;

namespace ReSharperPlugin.Alignator
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IAlignatorZone>;
}