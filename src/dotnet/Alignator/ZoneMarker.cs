using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using TBN.Alignator;

// Must be in the global namespace
[ZoneActivator]
public class ZoneActivator : IActivate<IAlignatorZone>;

namespace TBN.Alignator
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<IAlignatorZone>;
}