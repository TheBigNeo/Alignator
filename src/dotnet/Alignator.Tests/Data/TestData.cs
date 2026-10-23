// @formatter:empty_block_style together_same_line

// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedParameter.Local

namespace TBN.Alignator.Tests.Data;

internal abstract class AbstractDinosaur
{
    public int    Height      { get; set; }
    public string Description { get; set; }
}

internal sealed class Tyrannosaurus : AbstractDinosaur
{
    public Tyrannosaurus(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}

internal sealed class Velociraptor : AbstractDinosaur
{
    public Velociraptor(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}

internal sealed class Triceratops : AbstractDinosaur
{
    public Triceratops(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}

internal sealed class Brachiosaurus : AbstractDinosaur
{
    public Brachiosaurus(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}

internal enum Diet
{
    Carnivore = 1,
    Herbivore = 2,
}

public static class Caller
{
    public static void F1(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
    public static void F2(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
    public static void F3(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
    public static void F4(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}