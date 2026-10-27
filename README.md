# Alignator

Alignator is a plugin for **JetBrains Rider and ReSharper** that vertically aligns arguments in C# code,
padding whitespace so they line up in clean columns — making dense code easier to read.

## What can be aligned

For example, arguments of function calls:

**Before**

```csharp
Caller.F1("T-Rex", "Raptor", "Moros", "Triceratops");
Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
Caller.F2("Gallimimus", "Indor", "Carno", "Spino");
```

**After**

```csharp
Caller.F1("T-Rex",      "Raptor",        "Moros",         "Triceratops");
Caller.F1("Ankylo",     "Tyrannosaurus", "Indominus rex", "Brachio");
Caller.F2("Gallimimus", "Indor",         "Carno",         "Spino");
```

…or arguments of test attributes:

**Before**

```csharp
[TestCase("Tyrannosaurus", "Rex")]
[TestCase("Pachycephalosaurus", "Pachy")]
```

**After**

```csharp
[TestCase("Tyrannosaurus"     , "Rex"   )]
[TestCase("Pachycephalosaurus", "Pachy" )]
```

## Settings

All settings are found in the UI under
**Settings/Options → Code Editing → C# → Code Style → Others → Alignator align arguments**.

> **Important:** All Alignator alignments are driven by ReSharper's built-in column-alignment engine.
> They only take effect when the built-in `int_align_variables` switch is enabled (`true`).
> For convenience, the switch is also shown at the top of the Alignator settings group — it is the
> same value as in ReSharper's own formatting settings, so changing it in either place updates the
> single stored value.

| Setting (UI)                                        | Values                                               | Default        | .editorconfig key                                            |
|----------------------------------------------------|------------------------------------------------------|----------------|--------------------------------------------------------------|
| Align variables in columns *(built-in, required)*   | `true`, `false`                                      | —              | `int_align_variables`                                         |
| Align arguments in constructor                     | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` | `DoNotChange`  | `csharp_alignator_align_arguments_in_constructor`             |
| Align argument in function                         | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` | `DoNotChange`  | `csharp_alignator_align_arguments_in_function`                |
| Align arguments in attribute                       | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` | `DoNotChange`  | `csharp_alignator_align_arguments_in_attribute`               |
| Align closing parenthesis in attribute             | `true`, `false`                                      | `false`        | `csharp_alignator_align_closing_paren_in_attribute`           |
| Align argument names                               | `true`, `false`                                      | `false`        | `csharp_alignator_align_argument_names`                       |
| Align key-values in collection initializer         | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` | `DoNotChange`  | `csharp_alignator_align_key_values_in_collection_initializer` |
| Align closing braces in collection initializer     | `true`, `false`                                      | `false`        | `csharp_alignator_align_closing_braces_in_collection_initializer` |
| Align assignments in with expression               | `true`, `false`                                      | `false`        | `csharp_alignator_align_assignments_in_with_expression`      |

## Configuration via .editorconfig

The same settings can be configured in `.editorconfig`:

```ini
[*.cs]
# Built-in switch: Alignator alignments only take effect when this is true
int_align_variables = true

# Align arguments in constructor calls
csharp_alignator_align_arguments_in_constructor = space_after_comma

# Align arguments in function calls
csharp_alignator_align_arguments_in_function = space_after_comma

# Align arguments in attributes
csharp_alignator_align_arguments_in_attribute = space_after_comma

# Align closing parenthesis in attributes
csharp_alignator_align_closing_paren_in_attribute = true

# Align named-argument values in multi-line calls
csharp_alignator_align_argument_names = true

# Align key-value entries in dictionary-style collection initializers
csharp_alignator_align_key_values_in_collection_initializer = space_after_comma

# Align closing braces of dictionary-style entries
csharp_alignator_align_closing_braces_in_collection_initializer = true

# Align assignments in with-expression initializers
csharp_alignator_align_assignments_in_with_expression = true
```

Valid values for the `AlignPosition` settings: `do_not_change`, `space_before_comma`, `space_after_comma`.
Bool settings take `true` / `false`.

## Examples

Each setting below shows a before/after pair. The "after" state assumes code cleanup/formatting
with the respective setting enabled and `int_align_variables = true`.

### Align arguments in constructor

Before

```csharp
var tRex = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
var raptor = new Velociraptor("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
var trike = new Triceratops("Triceratops", "Herbivore", 9000, false, Diet.Herbivore) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null);
```

After (`space_after_comma`)

```csharp
var tRex = new Tyrannosaurus("T-Rex",           "Carnivore", 8000,  true,  Diet.Carnivore, "Jurassic Park");
var raptor = new Velociraptor("Raptor",         "Carnivore", 150,   true,  Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
var trike = new Triceratops("Triceratops",      "Herbivore", 9000,  false, Diet.Herbivore) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500,   true, Diet.Carnivore, null);
```

The same constructor calls in `space_before_comma` mode align the commas:

```csharp
var tRex = new Tyrannosaurus("T-Rex"          , "Carnivore", 8000 , true , Diet.Carnivore, "Jurassic Park");
var raptor = new Velociraptor("Raptor"        , "Carnivore", 150  , true , Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
var trike = new Triceratops("Triceratops"     , "Herbivore", 9000 , false, Diet.Herbivore) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500  , true, Diet.Carnivore, null);
```

### Align argument in function

Before

```csharp
Caller.F1("T-Rex", "Raptor", "Moros", "Triceratops");
Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
Caller.F2("Gallimimus", "Indor", "Carno", "Spino");
```

After (`space_after_comma`)

```csharp
Caller.F1("T-Rex",      "Raptor",        "Moros",         "Triceratops");
Caller.F1("Ankylo",     "Tyrannosaurus", "Indominus rex", "Brachio");
Caller.F2("Gallimimus", "Indor",         "Carno",         "Spino");
```

### Align arguments in attribute

Before

```csharp
[TestCase("Tyrannosaurus", "Rex")]
[TestCase("Velociraptor", "Raptor")]
[TestCase("Moros", "Moros")]
[TestCase("Pachycephalosaurus", "Pachy")]
```

After (`space_after_comma`)

```csharp
[TestCase("Tyrannosaurus",      "Rex")]
[TestCase("Velociraptor",       "Raptor")]
[TestCase("Moros",              "Moros")]
[TestCase("Pachycephalosaurus", "Pachy")]
```

### Align closing parenthesis in attribute

Before

```csharp
[TestCase("Tyrannosaurus", "Rex")]
[TestCase("Velociraptor", "Raptor")]
[TestCase("Moros", "Moros")]
[TestCase("Pachycephalosaurus", "Pachy")]
```

After (`true`)

```csharp
[TestCase("Tyrannosaurus",      "Rex"   )]
[TestCase("Velociraptor",       "Raptor")]
[TestCase("Moros",              "Moros" )]
[TestCase("Pachycephalosaurus", "Pachy" )]
```

### Align argument names

Before

```csharp
Caller.F1(o1: "T-Rex",
    o2: "Raptor",
    o3: "Triceratops");
```

After (`true`)

```csharp
Caller.F1(o1: "T-Rex",
    o2:       "Raptor",
    o3:       "Triceratops");
```

The same setting aligns named arguments of object creations (`new Velociraptor(o1: ...)` on separate lines)
and attribute named arguments in their only legal syntax, `Name = value`:

```csharp
[TestCase(o1 = "Tyrannosaurus",
    o2 =       "Rex")]
```

Named arguments are only padded when they start their own line: single-line calls stay untouched,
and positional arguments in the same call are never modified. When a call uses named arguments,
the comma alignment rules above (constructor, function, attribute) stand back for that call —
including for the positional arguments of a mixed list — so comma alignment and argument-name
alignment never pad the same lines twice.

### Align key-values in collection initializer

Before

```csharp
Dictionary<string, string?> indominusRexFacts = new()
{
  {"Name", "Indominus Rex"},
  {"Diet", "Carnivore"},
  {"Enclosure", "Paddock 11"},
  {"Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},
};
```

After (`space_after_comma`)

```csharp
Dictionary<string, string?> indominusRexFacts = new()
{
  {"Name",      "Indominus Rex"},
  {"Diet",      "Carnivore"},
  {"Enclosure", "Paddock 11"},
  {"Genome",    "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},
};
```

Only dictionary-style entries with two or more values (e.g. `{"Key", Value}`) are aligned;
simple `{expr}` entries are never touched. Entries are only aligned when they start their own line.

### Align closing braces in collection initializer

Before

```csharp
Dictionary<string, string?> indominusRexFacts = new()
{
  {"Name", "Indominus Rex"},
  {"Diet", "Carnivore"},
  {"Enclosure", "Paddock 11"},
  {"Genome", "T-Rex, Velociraptor, Cuttlefish, Tree Frog"},
};
```

After (`true`; shown together with `space_after_comma` for keys and values)

```csharp
Dictionary<string, string?> indominusRexFacts = new()
{
  {"Name",      "Indominus Rex"                              },
  {"Diet",      "Carnivore"                                  },
  {"Enclosure", "Paddock 11"                                 },
  {"Genome",    "T-Rex, Velociraptor, Cuttlefish, Tree Frog" },
};
```

### Align assignments in with expression

Before

```csharp
DinosaurRecord indominusRex = GetDinosaurRecord(ParkSettings.Instance)
  with
  {
    Name = "Indominus Rex",
    Diet = "Carnivore",
    PaddockNumber = 11,
    DangerLevel = DangerLevel.Extreme,
    ThermalCamouflageEnabled = true,
  };
```

After (`true`)

```csharp
DinosaurRecord indominusRex = GetDinosaurRecord(ParkSettings.Instance)
  with
  {
    Name                     = "Indominus Rex",
    Diet                     = "Carnivore",
    PaddockNumber            = 11,
    DangerLevel              = DangerLevel.Extreme,
    ThermalCamouflageEnabled = true,
  };
```

---

> **For developers and AI agents:** everything about how this project works — architecture, build,
> versioning, publishing, and installation — is documented in [DEVELOPERS.md](DEVELOPERS.md).
