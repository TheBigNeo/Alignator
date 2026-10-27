# Alignator

Alignator is a plugin for **JetBrains Rider and ReSharper** that pads whitespace in C# code so that related elements line up in clean columns:
arguments of constructor, function, and attribute calls, values of named arguments, dictionary-style entries in collection initializers, and assignments in `with` expressions.

## What can be aligned

For example, arguments of function calls:

**Before**

```csharp
Caller.Function1("T-Rex",  "Raptor",        "Moros",         "Triceratops");
Caller.ThatIsADifferentFunction("Gallimimus", "Indor", "Carno", "Spino");
Caller.Function1("Veloci", "Baryo", "Moso", "Dilophosaurus");
```

**After**

```csharp
Caller.Function1("T-Rex",                     "Raptor", "Moros", "Triceratops");
Caller.ThatIsADifferentFunction("Gallimimus", "Indor",  "Carno", "Spino");
Caller.Function1("Veloci",                    "Baryo",  "Moso",  "Dilophosaurus");
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

### Four alignment modes

The argument settings (constructor, function, attribute) offer four modes:

- **SpaceAfterComma** — pads after each comma, so arguments start in the same column
- **SpaceBeforeComma** — pads before each comma, so the commas line up vertically
- **SpaceBeforeParenthese** — pads before the opening parenthesis, so the parentheses (and with them the first arguments) line up vertically
- **SpaceAfterParenthese** — pads after the opening parenthesis, so the first arguments line up vertically

In both paren modes the arguments after the first are padded like in `SpaceAfterComma`.

```csharp
// SpaceBeforeComma
Caller.F1("T-Rex"     , "Raptor"       , "Moros"        , "Triceratops");
Caller.F2("Gallimimus", "Indor"        , "Carno"        , "Spino");
Caller.F1("Ankylo"    , "Tyrannosaurus", "Indominus rex", "Brachio");

// SpaceBeforeParenthese
Caller.Function1(               "T-Rex",      "Raptor", "Moros", "Triceratops");
Caller.ThatIsADifferentFunction("Gallimimus", "Indor",  "Carno", "Spino");
Caller.Function1(               "Veloci",     "Baryo",  "Moso",  "Dilophosaurus");

// SpaceAfterParenthese
Caller.Function1(               "T-Rex",      "Raptor", "Moros", "Triceratops");
Caller.ThatIsADifferentFunction("Gallimimus", "Indor",  "Carno", "Spino");
Caller.Function1(               "Veloci",     "Baryo",  "Moso",  "Dilophosaurus");
```

> **Note on SpaceBeforeComma:** it is rarely used as the final code style in production.
> But it is handy as a temporary working style — quickly switch to it, make bulk edits where the visible comma columns help, then switch back to your usual style.

> Whether closing brackets are aligned too is decided by separate settings:
> `csharp_alignator_align_closing_paren_in_attribute` for the `)` of attributes, and `csharp_alignator_align_closing_braces_in_collection_initializer` for the `}` of dictionary-style entries.

## Settings

Open **[File | Settings | Editor | Code Style | C#](jetbrains://Rider/settings?name=Editor--Code+Style--C%23)**, tab **Others**, section **Alignator align arguments**.
In Visual Studio with ReSharper: **ReSharper | Options | Code Editing | C# | Code Style | Others**.

> **Important:** All Alignator alignments are driven by ReSharper's built-in column-alignment engine.
> They only take effect when the built-in `int_align_variables` switch is enabled (`true`).
> For convenience, the switch is also shown at the top of the Alignator settings group —
> it is the same value as in ReSharper's own formatting settings, so changing it in either place updates the single stored value.

| Setting (UI)                                      | Values                                               |
|---------------------------------------------------|------------------------------------------------------|
| Align variables in columns *(built-in, required)* | `true`, `false`                                      |
| Align arguments in constructor                    | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`, `SpaceBeforeParenthese`, `SpaceAfterParenthese` |
| Align argument in function                        | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`, `SpaceBeforeParenthese`, `SpaceAfterParenthese` |
| Align arguments in attribute                      | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`, `SpaceBeforeParenthese`, `SpaceAfterParenthese` |
| Align closing parenthesis in attribute            | `true`, `false`                                      |
| Align argument names                              | `true`, `false`                                      |
| Align key-values in collection initializer        | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` |
| Align closing braces in collection initializer    | `true`, `false`                                      |
| Align assignments in with expression              | `true`, `false`                                      |

The `.editorconfig` key for each setting is shown in the collapsed example below and in [DEVELOPERS.md](DEVELOPERS.md).

## Configuration via .editorconfig

The same settings can be configured in `.editorconfig`:

<details>
<summary>.editorconfig example (click to expand)</summary>

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

</details>

Valid values for the `AlignPosition` settings: `do_not_change`, `space_before_comma`, `space_after_comma`, `space_before_parenthese`, `space_after_parenthese`. Bool settings take `true` / `false`.
The two paren values apply to the constructor, function and attribute settings only - for `csharp_alignator_align_key_values_in_collection_initializer` they have no effect (dictionary-style entries have no parentheses).

## Examples

> All examples assume `int_align_variables = true`.
> This built-in switch is required for every Alignator alignment — it also aligns the `=` of consecutive variable declarations, as visible in the constructor example.

Each setting below shows a before/after pair for a formatting run with the respective setting enabled.

### Align arguments in constructor

> `csharp_alignator_align_arguments_in_constructor`

Before

```csharp
var tRex = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
var raptor = new Velociraptor("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
var trike = new Triceratops("Triceratops", "Herbivore", 9000, false, Diet.Herbivore, null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null);
```

After (`space_after_comma`)

```csharp
var tRex   = new Tyrannosaurus("T-Rex",         "Carnivore", 8000, true,  Diet.Carnivore, "Jurassic Park");
var raptor = new Velociraptor("Raptor",         "Carnivore", 150,  true,  Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
var trike  = new Triceratops("Triceratops",     "Herbivore", 9000, false, Diet.Herbivore, null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500,  true,  Diet.Carnivore, null);
```

The same constructor calls in `space_before_comma` mode align the commas:

```csharp
var tRex   = new Tyrannosaurus("T-Rex"        , "Carnivore", 8000, true , Diet.Carnivore, "Jurassic Park");
var raptor = new Velociraptor("Raptor"        , "Carnivore", 150 , true , Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
var trike  = new Triceratops("Triceratops"    , "Herbivore", 9000, false, Diet.Herbivore, null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500 , true , Diet.Carnivore, null);
```

The first argument can be aligned too. In `space_after_parenthese` mode the parentheses stay where they are and the first arguments share one column:

```csharp
var tRex   = new Tyrannosaurus(  "T-Rex",         "Carnivore", 8000, true,  Diet.Carnivore, "Jurassic Park");
var raptor = new Velociraptor( "Raptor",        "Carnivore", 150,  true,  Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
var trike  = new Triceratops(   "Triceratops",   "Herbivore", 9000, false, Diet.Herbivore, null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500,  true,  Diet.Carnivore, null);
```

In `space_before_parenthese` mode the opening parentheses line up instead, which puts the first arguments in one column as well:

```csharp
var tRex   = new Tyrannosaurus("T-Rex",         "Carnivore", 8000, true,  Diet.Carnivore, "Jurassic Park");
var raptor = new Velociraptor ("Raptor",        "Carnivore", 150,  true,  Diet.Carnivore, "Jurassic Park 2") { Height = 2 };
var trike  = new Triceratops  ("Triceratops",   "Herbivore", 9000, false, Diet.Herbivore, null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500,  true,  Diet.Carnivore, null);
```

Both paren modes align the arguments after the first like `space_after_comma`.

### Align argument in function

> `csharp_alignator_align_arguments_in_function`

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

The paren modes align the first argument instead of the commas - `space_before_parenthese` pads before the opening parenthesis, `space_after_parenthese` pads after it. The remaining arguments are aligned like `space_after_comma`:

Before

```csharp
Caller.Function1("T-Rex", "Raptor", "Moros", "Triceratops");
Caller.ThatIsADifferentFunction("Gallimimus", "Indor", "Carno", "Spino");
Caller.Function1("Veloci", "Baryo", "Moso", "Dilophosaurus");
```

After (`space_before_parenthese`)

```csharp
Caller.Function1(               "T-Rex",      "Raptor", "Moros", "Triceratops");
Caller.ThatIsADifferentFunction("Gallimimus", "Indor",  "Carno", "Spino");
Caller.Function1(               "Veloci",     "Baryo",  "Moso",  "Dilophosaurus");
```

After (`space_after_parenthese`)

```csharp
Caller.Function1(               "T-Rex",      "Raptor", "Moros", "Triceratops");
Caller.ThatIsADifferentFunction("Gallimimus", "Indor",  "Carno", "Spino");
Caller.Function1(               "Veloci",     "Baryo",  "Moso",  "Dilophosaurus");
```

### Align arguments in attribute

> `csharp_alignator_align_arguments_in_attribute`

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

> `csharp_alignator_align_closing_paren_in_attribute`

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

> `csharp_alignator_align_argument_names`

Before

```csharp
Caller.F2(aShortName: "T-Rex",
    aVeryVeryLoooooooongName: "Raptor",
    aNormalName: "Triceratops");
```

After (`true`)

```csharp
Caller.F2(aShortName:       "T-Rex",
    aVeryVeryLoooooooongName: "Raptor",
    aNormalName:              "Triceratops");
```

The same setting aligns named arguments of object creations (`new Velociraptor(aShortName: ...)` on separate lines)
and attribute named arguments in their only legal syntax, `Name = value`:

```csharp
[TestCase(aShortName =         "Tyrannosaurus",
    aVeryVeryLoooooooongName = "Rex")]
```

Named arguments are only padded when they start their own line: single-line calls stay untouched, and positional arguments in the same call are never modified.
When a call uses named arguments, the comma alignment rules above (constructor, function, attribute) stand back for that call — including for the positional arguments of a mixed list —
so comma alignment and argument-name alignment never pad the same lines twice.

### Align key-values in collection initializer

> `csharp_alignator_align_key_values_in_collection_initializer`

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

Only dictionary-style entries with two or more values (e.g. `{"Key", Value}`) are aligned; simple `{expr}` entries are never touched.
Entries are only aligned when they start their own line.

### Align closing braces in collection initializer

> `csharp_alignator_align_closing_braces_in_collection_initializer`

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

> `csharp_alignator_align_assignments_in_with_expression`

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

> **For developers and AI agents:** everything about how this project works — architecture, build, versioning, publishing, and installation —
> is documented in [DEVELOPERS.md](DEVELOPERS.md).
