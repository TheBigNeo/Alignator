# Alignator

Alignator is a plugin for **JetBrains Rider and ReSharper** that pads whitespace in C# code so that related elements line up in clean columns:
arguments of constructor, function, and attribute calls, values of named arguments, dictionary-style entries in collection initializers, and assignments in `with` expressions.

## What can be aligned

For example, arguments of function calls:

**Before**

```csharp
Caller.FeedDinosaurs("T-Rex", "Raptor", "Moros", "Triceratops");
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor", "Carno", "Spino");
Caller.FeedDinosaurs("Veloci", "Baryo", "Moso", "Dilophosaurus");
```

**After**

```csharp
Caller.FeedDinosaurs("T-Rex",                      "Raptor", "Moros", "Triceratops");
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor",  "Carno", "Spino");
Caller.FeedDinosaurs("Veloci",                     "Baryo",  "Moso",  "Dilophosaurus");
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

### Alignment modes

The argument settings (constructor, function, attribute) come in two independent groups:
one for the arguments after the first, one for the first argument. The two settings work
together - they never exclude each other, and each can also be enabled on its own.

Arguments after the first (`csharp_alignator_align_*_arguments`):

- **SpaceAfterComma** — pads after each comma, so arguments start in the same column
- **SpaceBeforeComma** — pads before each comma, so the commas line up vertically

The first argument (`csharp_alignator_align_*_arguments_first`):

- **SpaceBeforeParenthese** — pads before the opening parenthesis, so the parentheses (and with them the first arguments) line up vertically
- **SpaceAfterParenthese** — pads after the opening parenthesis, so the first arguments line up vertically

```csharp
// AlignPosition = SpaceBeforeComma
Caller.FeedDinosaurs("T-Rex"          , "Raptor"       , "Moros"        , "Triceratops");
Caller.CountHerdInPaddock("Gallimimus", "Indor"        , "Carno"        , "Spino");
Caller.FeedDinosaurs("Ankylo"         , "Tyrannosaurus", "Indominus rex", "Brachio");

// FirstArgumentPosition = SpaceBeforeParenthese
Caller.FeedDinosaurs                ("T-Rex",      "Raptor", "Moros", "Triceratops");
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor",  "Carno", "Spino");
Caller.FeedDinosaurs                ("Veloci",     "Baryo",  "Moso",  "Dilophosaurus");

// FirstArgumentPosition = SpaceAfterParenthese
Caller.FeedDinosaurs(                "T-Rex",      "Raptor", "Moros", "Triceratops");
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor",  "Carno", "Spino");
Caller.FeedDinosaurs(                "Veloci",     "Baryo",  "Moso",  "Dilophosaurus");

// FirstArgumentPosition = SpaceAfterParenthese + AlignPosition = SpaceBeforeComma
Caller.FeedDinosaurs(                "T-Rex"     , "Raptor", "Moros", "Triceratops");
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor" , "Carno", "Spino");
Caller.FeedDinosaurs(                "Veloci"    , "Baryo" , "Moso" , "Dilophosaurus");
```

> **Note on SpaceBeforeComma:** it is rarely used as the final code style in production.
> But it is handy as a temporary working style — quickly switch to it, make bulk edits where the visible comma columns help, then switch back to your usual style.

> Whether closing brackets are aligned too is decided by separate settings:
> `csharp_alignator_align_attribute_arguments_closing_paren` for the `)` of attributes, and `csharp_alignator_align_collection_initializer_closing_braces` for the `}` of dictionary-style entries.

## Settings

Open **[File | Settings | Editor | Code Style | C#](jetbrains://Rider/settings?name=Editor--Code+Style--C%23)**, tab **Others**, section **Alignator align arguments**.
In Visual Studio with ReSharper: **ReSharper | Options | Code Editing | C# | Code Style | Others**.

> **Important:** All Alignator alignments are driven by ReSharper's built-in column-alignment engine.
> They only take effect when the built-in `int_align_variables` switch is enabled (`true`).
> For convenience, the switch is also shown at the top of the Alignator settings group —
> it is the same value as in ReSharper's own formatting settings, so changing it in either place updates the single stored value.

| Setting (UI)                                      | Values                                                         |
|---------------------------------------------------|----------------------------------------------------------------|
| Align variables in columns *(built-in, required)* | `true`, `false`                                                |
| Align arguments in constructor                    | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`           |
| Align argument in function                        | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`           |
| Align arguments in attribute                      | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`           |
| Align first argument in constructor               | `DoNotChange`, `SpaceBeforeParenthese`, `SpaceAfterParenthese` |
| Align first argument in function                  | `DoNotChange`, `SpaceBeforeParenthese`, `SpaceAfterParenthese` |
| Align first argument in attribute                 | `DoNotChange`, `SpaceBeforeParenthese`, `SpaceAfterParenthese` |
| Align closing parenthesis in attribute            | `true`, `false`                                                |
| Align closing parenthesis in function             | `true`, `false`                                                |
| Align argument names                              | `true`, `false`                                                |
| Align key-values in collection initializer        | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`           |
| Align closing braces in collection initializer    | `true`, `false`                                                |
| Align assignments in with expression              | `true`, `false`                                                |

The `.editorconfig` key for each setting is shown in the collapsed example below and in [DEVELOPERS.md](DEVELOPERS.md).

## Configuration via .editorconfig

The same settings can be configured in `.editorconfig`:

<details>
<summary>.editorconfig example (click to expand)</summary>

```ini
[*.cs]
# Built-in switch: Alignator alignments only take effect when this is true
int_align_variables                                          = true

# Constructor calls
csharp_alignator_align_constructor_arguments                 = space_after_comma       # Align arguments in constructor calls
csharp_alignator_align_constructor_arguments_first           = space_before_parenthese # Align the first argument of constructor calls

# Function calls
csharp_alignator_align_function_arguments                    = space_after_comma # Align arguments in function calls
csharp_alignator_align_function_arguments_first              = do_not_change     # Align the first argument of function calls
csharp_alignator_align_function_arguments_closing_paren      = true              # Align closing parenthesis in functions

# Attributes
csharp_alignator_align_attribute_arguments                   = space_after_comma # Align arguments in attributes
csharp_alignator_align_attribute_arguments_first             = do_not_change     # Align the first argument of attributes
csharp_alignator_align_attribute_arguments_closing_paren     = true              # Align closing parenthesis in attributes

# Named arguments
csharp_alignator_align_arguments_named                       = true # Align named-argument values in multi-line calls

# Collection initializers
csharp_alignator_align_collection_initializer_key_values     = space_after_comma # Align key-value entries in dictionary-style collection initializers
csharp_alignator_align_collection_initializer_closing_braces = true              # Align closing braces of dictionary-style entries

# With expressions
csharp_alignator_align_with_expression_assignments           = true # Align assignments in with-expression initializers
```

</details>

Valid values for the `AlignPosition` settings: `do_not_change`, `space_before_comma`, `space_after_comma`. Bool settings take `true` / `false`.
Valid values for the `FirstArgumentPosition` settings: `do_not_change`, `space_before_parenthese`, `space_after_parenthese`.
Each first-argument setting combines freely with its comma counterpart — for example `csharp_alignator_align_constructor_arguments_first = space_before_parenthese` and `csharp_alignator_align_constructor_arguments = space_before_comma` can both be active.
The collection-initializer setting has no first-argument counterpart (dictionary-style entries have no parentheses).

## Examples

> All examples assume `int_align_variables = true`.
> This built-in switch is required for every Alignator alignment — it also aligns the `=` of consecutive variable declarations, as visible in the constructor example.

Each setting below shows a before/after pair for a formatting run with the respective setting enabled.

### Align arguments in constructor

> `csharp_alignator_align_constructor_arguments`

Before

```csharp
var tRex = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
var moros = new Moros("Moros", "Carnivore", 70, true, Diet.Carnivore, null);
var raptor = new Velociraptor("Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
var trike = new Triceratops("Triceratops", "Herbi", 9000, false, Diet.Herbi, null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null);
```

After (`space_after_comma`)

```csharp
var tRex   = new Tyrannosaurus("T-Rex",          "Carnivore", 8000, true,  Diet.Carnivore, "Jurassic Park");
var moros  = new Moros("Moros",                  "Carnivore", 70,   true,  Diet.Carnivore, null);
var raptor = new Velociraptor("Raptor",          "Carnivore", 150,  true,  Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
var trike  = new Triceratops("Triceratops",      "Herbi",     9000, false, Diet.Herbi,     null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus",  "Carnivore", 500,  true,  Diet.Carnivore, null);
```

The same constructor calls in `space_before_comma` mode align the commas:

```csharp
var tRex   = new Tyrannosaurus("T-Rex"        , "Carnivore", 8000, true , Diet.Carnivore, "Jurassic Park");
var moros  = new Moros("Moros"                , "Carnivore", 70  , true , Diet.Carnivore, null);
var raptor = new Velociraptor("Raptor"        , "Carnivore", 150 , true , Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
var trike  = new Triceratops("Triceratops"    , "Herbi"    , 9000, false, Diet.Herbi    , null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500 , true , Diet.Carnivore, null);
```

### Align first argument in constructor

> `csharp_alignator_align_constructor_arguments_first`

This setting is independent of the comma setting above — both can be enabled at the same time.
In `space_after_parenthese` mode the parentheses stay where they are and the first arguments share one column:

```csharp
var tRex   = new Tyrannosaurus( "T-Rex",          "Carnivore", 8000, true,  Diet.Carnivore, "Jurassic Park");
var moros  = new Moros(         "Moros",          "Carnivore", 70,   true,  Diet.Carnivore, null);
var raptor = new Velociraptor(  "Raptor",         "Carnivore", 150,  true,  Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
var trike  = new Triceratops(   "Triceratops",    "Herbi",     9000, false, Diet.Herbi,     null) { Height = 3 };
var diloph = new Brachiosaurus( "Dilophosaurus",  "Carnivore", 500,  true,  Diet.Carnivore, null);
```

In `space_before_parenthese` mode the opening parentheses line up instead, which puts the first arguments in one column as well:

```csharp
var tRex   = new Tyrannosaurus("T-Rex",          "Carnivore", 8000, true,  Diet.Carnivore, "Jurassic Park");
var moros  = new Moros(        "Moros",          "Carnivore", 70,   true,  Diet.Carnivore, null);
var raptor = new Velociraptor( "Raptor",         "Carnivore", 150,  true,  Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
var trike  = new Triceratops(  "Triceratops",    "Herbi",     9000, false, Diet.Herbi,     null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus",  "Carnivore", 500,  true,  Diet.Carnivore, null);
```

With the comma setting left at `do_not_change`, only the first argument is padded - the arguments
after the first keep their original spacing:

```csharp
var tRex   = new Tyrannosaurus("T-Rex", "Carnivore", 8000, true, Diet.Carnivore, "Jurassic Park");
var moros  = new Moros(        "Moros", "Carnivore", 70, true, Diet.Carnivore, null);
var raptor = new Velociraptor( "Raptor", "Carnivore", 150, true, Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
var trike  = new Triceratops(  "Triceratops", "Herbi", 9000, false, Diet.Herbi, null) { Height = 3 };
var diloph = new Brachiosaurus("Dilophosaurus", "Carnivore", 500, true, Diet.Carnivore, null);
```

And both settings combined - first argument after the parenthesis, commas before each comma:

```csharp
var tRex   = new Tyrannosaurus( "T-Rex"        , "Carnivore", 8000, true , Diet.Carnivore, "Jurassic Park");
var moros  = new Moros(         "Moros"        , "Carnivore", 70  , true , Diet.Carnivore, null);
var raptor = new Velociraptor(  "Raptor"       , "Carnivore", 150 , true , Diet.Carnivore, "Jurassic Park 2 - The Lost World") { Height = 2 };
var trike  = new Triceratops(   "Triceratops"  , "Herbi"    , 9000, false, Diet.Herbi    , null) { Height = 3 };
var diloph = new Brachiosaurus( "Dilophosaurus", "Carnivore", 500 , true , Diet.Carnivore, null);
```

### Align argument in function

> `csharp_alignator_align_function_arguments`

Before

```csharp
Caller.FeedDinosaurs("T-Rex", "Raptor", "Moros", "Triceratops");
Caller.FeedDinosaurs("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
Caller.CountHerdInPaddock("Gallimimus", "Indor", "Carno", "Spino");
```

After (`space_after_comma`)

```csharp
Caller.FeedDinosaurs("T-Rex",           "Raptor",        "Moros",         "Triceratops");
Caller.FeedDinosaurs("Ankylo",          "Tyrannosaurus", "Indominus rex", "Brachio");
Caller.CountHerdInPaddock("Gallimimus", "Indor",         "Carno",         "Spino");
```

### Align first argument in function

> `csharp_alignator_align_function_arguments_first`

This setting pads the first argument instead of the commas - `space_before_parenthese` pads
before the opening parenthesis, `space_after_parenthese` pads after it. It is independent of the
comma setting, which keeps aligning the arguments after the first:

Before

```csharp
Caller.FeedDinosaurs("T-Rex", "Raptor", "Moros", "Triceratops");
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor", "Carno", "Spino");
Caller.FeedDinosaurs("Veloci", "Baryo", "Moso", "Dilophosaurus");
```

After (`space_before_parenthese`, shown together with `space_after_comma` for the remaining arguments)

```csharp
Caller.FeedDinosaurs                ("T-Rex",      "Raptor", "Moros", "Triceratops");
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor",  "Carno", "Spino");
Caller.FeedDinosaurs                ("Veloci",     "Baryo",  "Moso",  "Dilophosaurus");
```

After (`space_after_parenthese`, shown together with `space_after_comma` for the remaining arguments)

```csharp
Caller.FeedDinosaurs(                "T-Rex",      "Raptor", "Moros", "Triceratops");
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor",  "Carno", "Spino");
Caller.FeedDinosaurs(                "Veloci",     "Baryo",  "Moso",  "Dilophosaurus");
```

### Align arguments in attribute

> `csharp_alignator_align_attribute_arguments`

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

### Align first argument in attribute

> `csharp_alignator_align_attribute_arguments_first`

Attribute calls often have very different callee lengths, so the first-argument alignment is
especially visible there. Like the other first-argument settings it combines freely with the
attribute comma setting.

Before

```csharp
[TyrannosaurusTestCase("Tyrannosaurus", "Rex")]
[RaptorTest("Velociraptor", "Raptor")]
[Test("Moros", "Moros")]
[PachycephalosaurusTestCase("Pachycephalosaurus", "Pachy")]
```

After (`space_after_parenthese`)

```csharp
[TyrannosaurusTestCase(     "Tyrannosaurus",      "Rex")]
[RaptorTest(                "Velociraptor",       "Raptor")]
[Test(                      "Moros",              "Moros")]
[PachycephalosaurusTestCase("Pachycephalosaurus", "Pachy")]
```

After (`space_before_parenthese`)

```csharp
[TyrannosaurusTestCase     ("Tyrannosaurus",      "Rex")]
[RaptorTest                ("Velociraptor",       "Raptor")]
[Test                      ("Moros",              "Moros")]
[PachycephalosaurusTestCase("Pachycephalosaurus", "Pachy")]
```

### Align closing parenthesis in attribute

> `csharp_alignator_align_attribute_arguments_closing_paren`

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

### Align closing parenthesis in function

> `csharp_alignator_align_function_arguments_closing_paren`

Aligns the closing parentheses of consecutive `Foo(...)` invocations of one block.
It combines freely with the comma and first-argument settings, and a closing parenthesis
wrapped onto its own line is never padded.

Before

```csharp
Caller.FeedDinosaurs("T-Rex", "Raptor", "Moros", "Triceratops");
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor", "Carno", "Spino");
Caller.FeedDinosaurs("Veloci", "Baryo", "Moso", "Dilophosaurus");
```

After (`true`)

```csharp
Caller.FeedDinosaurs("T-Rex", "Raptor", "Moros", "Triceratops"              );
Caller.TransportDinosaursToIslaSorna("Gallimimus", "Indor", "Carno", "Spino");
Caller.FeedDinosaurs("Veloci", "Baryo", "Moso", "Dilophosaurus"             );
```

### Align argument names

> `csharp_alignator_align_arguments_named`

Before

```csharp
Caller.TrackPack(rex: "T-Rex",
    pachycephalosaurus: "Raptor",
    gallimimus: "Triceratops");
```

After (`true`)

```csharp
Caller.TrackPack(rex:  "T-Rex",
    pachycephalosaurus: "Raptor",
    gallimimus:        "Triceratops");
```

The same setting aligns named arguments of object creations (`new Velociraptor(rex: ...)` on separate lines)
and attribute named arguments in their only legal syntax, `Name = value`:

```csharp
[TestCase(rex =         "Tyrannosaurus",
    pachycephalosaurus = "Rex")]
```

Named arguments are only padded when they start their own line: single-line calls stay untouched, and positional arguments in the same call are never modified.
When a call uses named arguments, the comma alignment rules above (constructor, function, attribute) stand back for that call — including for the positional arguments of a mixed list —
so comma alignment and argument-name alignment never pad the same lines twice.

### Align key-values in collection initializer

> `csharp_alignator_align_collection_initializer_key_values`

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

> `csharp_alignator_align_collection_initializer_closing_braces`

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

> `csharp_alignator_align_with_expression_assignments`

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
