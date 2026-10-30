// @formatter:empty_block_style together_same_line

// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedParameter.Local

using System;

namespace TBN.Alignator.Tests.Data;

internal abstract class AbstractDinosaur
{
    public int Height { get; set; }
    public string Description { get; set; }
}

internal sealed class Tyrannosaurus : AbstractDinosaur
{
    public Tyrannosaurus(object name, object diet = null, object weight = null, object isCarnivore = null, object dietType = null, object park = null) { }
}

internal sealed class Velociraptor : AbstractDinosaur
{
    public Velociraptor(object name, object diet = null, object weight = null, object isCarnivore = null, object dietType = null, object park = null) { }
}

internal sealed class Triceratops : AbstractDinosaur
{
    public Triceratops(object name, object diet = null, object weight = null, object isCarnivore = null, object dietType = null, object park = null) { }
}

internal sealed class Brachiosaurus : AbstractDinosaur
{
    public Brachiosaurus(object name, object diet = null, object weight = null, object isCarnivore = null, object dietType = null, object park = null) { }
}

internal sealed class Moros : AbstractDinosaur
{
    public Moros(object name, object diet = null, object weight = null, object isCarnivore = null, object dietType = null, object park = null) { }
}

internal sealed class Spinosaurus : AbstractDinosaur
{
    public Spinosaurus(object name, object diet = null, object weight = null, object isCarnivore = null, object dietType = null, object park = null) { }
}

internal enum Diet
{
    Carnivore = 1,
    Herbi = 2,
}

public static class Caller
{
    public static void Scan(object rex, object raptor = null, object spino = null, object baryo = null, object moso = null, object galli = null) { }
    public static void TrackPack(object rex, object pachycephalosaurus = null, object gallimimus = null) { }
    public static void FeedDinosaurs(object rex, object raptor = null, object spino = null, object baryo = null, object moso = null, object galli = null) { }
    public static void CountHerdInPaddock(object rex, object raptor = null, object spino = null, object baryo = null, object moso = null, object galli = null) { }
    public static void TransportDinosaursToIslaSorna(object rex, object raptor = null, object spino = null, object baryo = null, object moso = null, object galli = null) { }
}

internal sealed class DinosaurPark
{
    private Tyrannosaurus _tRex;
    private Moros _moros;
    private Velociraptor _raptor;
    public Triceratops Trice { get; private set; }
    public Brachiosaurus Brachio { get; }

    public DinosaurPark(int sortIndex)
    {
        _tRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
        _moros  = new Moros("Moros", "Carnivore", 70, true, Diet.Carnivore, null);
        _raptor = new Velociraptor("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
        Trice   = new Triceratops("Trice", "Herbi", 9000, false, Diet.Herbi) { Height                                             = 3 };
        Brachio = new Brachiosaurus("Brachio", "Herbi", 6000, false, 250) { Description                                           = "Long neck" };
    }
}

internal enum DangerLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Extreme = 4,
}

internal record DinosaurRecord(string Name, string Diet, int PaddockNumber, DangerLevel DangerLevel, bool ThermalCamouflageEnabled, int Speed = 0);

internal record FeedingSchedule(string Time, string Food, int Quantity, string Recipient);

internal sealed class ParkSettings
{
    public static ParkSettings Instance { get; } = new();
    private ParkSettings() { }
}

internal static class RecordFactory
{
    public static DinosaurRecord GetDinosaurRecord(object source) => new("Echo", "Carnivore", 0, DangerLevel.Low, false);
    public static FeedingSchedule GetFeedingSchedule(object source) => new("09:00", "Goat", 1, "T-Rex");
}

// Attribute named arguments are matched case-sensitively,
// so the properties carry the lowercase names the test data writes.
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class TestCaseAttribute : Attribute
{
    public TestCaseAttribute(params object[] args) { }

    public object name { get; set; }
    public object diet { get; set; }
    public object park { get; set; }
    public object rex { get; set; }
    public object raptor { get; set; }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class DinoAttribute : Attribute
{
    public DinoAttribute(params object[] args) { }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class TyrannosaurusTestCaseAttribute : Attribute
{
    public TyrannosaurusTestCaseAttribute(params object[] args) { }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class RaptorTestAttribute : Attribute
{
    public RaptorTestAttribute(params object[] args) { }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class PachycephalosaurusTestCaseAttribute : Attribute
{
    public PachycephalosaurusTestCaseAttribute(params object[] args) { }
}
