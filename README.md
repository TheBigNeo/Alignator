# Alignator for Rider and ReSharper

[![Rider](https://img.shields.io/jetbrains/plugin/v/RIDER_PLUGIN_ID.svg?label=Rider&colorB=0A7BBB&style=for-the-badge&logo=rider)](https://plugins.jetbrains.com/plugin/RIDER_PLUGIN_ID)
[![ReSharper](https://img.shields.io/jetbrains/plugin/v/RESHARPER_PLUGIN_ID.svg?label=ReSharper&colorB=0A7BBB&style=for-the-badge&logo=resharper)](https://plugins.jetbrains.com/plugin/RESHARPER_PLUGIN_ID)

Alignator is a code formatter plugin that vertically aligns arguments in C# constructor calls and function invocations,
making your code easier to read.

## Features

- **Align arguments in constructor calls** (`new Type(...)`)
- **Align arguments in function calls** (`Foo(...)`)
- **Align arguments in attributes** (`[TestCase("Tyrannosaurus", "Rex")]`), with optional closing-parenthesis alignment
- Two alignment modes:
    - `SpaceAfterComma` — pads space after each comma so arguments align in columns
    - `SpaceBeforeComma` — pads space before each comma so commas align vertically

## Settings

Settings are available in **Settings → Code Editing → C# → Code Style → Others → Alignator align arguments**.

> **Important:** All Alignator alignments are driven by ReSharper's built-in column-alignment engine.
> They only take effect when the built-in `int_align_variables` switch is enabled (`true`).
> For convenience, the switch is also shown at the top of the Alignator settings group — it is the
> same setting value as in ReSharper's own formatting settings, so changing it in either place
> updates the single stored value.

| Setting                                | Description                                                                            | Values                                               | .editorconfig                                       |
|----------------------------------------|----------------------------------------------------------------------------------------|------------------------------------------------------|-----------------------------------------------------|
| Align variables in columns (built-in)  | ReSharper's column-alignment engine switch — **required** for all Alignator alignments | `true`, `false`                                      | `int_align_variables`                               |
| Align arguments in constructor         | Alignment mode for `new Type(...)` calls                                               | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` | `csharp_alignator_align_arguments_in_constructor`   |
| Align argument in function             | Alignment mode for `Foo(...)` calls                                                    | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` | `csharp_alignator_align_arguments_in_function`      |
| Align arguments in attribute           | Alignment mode for attribute arguments                                                 | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` | `csharp_alignator_align_arguments_in_attribute`     |
| Align closing parenthesis in attribute | Aligns `)` of attributes in one column                                                 | `true`, `false`                                      | `csharp_alignator_align_closing_paren_in_attribute` |

## .editorconfig

The plugin settings can also be configured via `.editorconfig`:

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
```

Valid values: `do_not_change`, `space_before_comma`, `space_after_comma`

## Example

**Before:**

```csharp
Caller.F1("T-Rex",  "Raptor",        "Moros",         "Triceratops");
Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
Caller.F2("Gallimimus", "Indor", "Carno", "Spino");
```

**After (SpaceAfterComma):**

```csharp
Caller.F1("T-Rex",      "Raptor",        "Moros",         "Triceratops");
Caller.F1("Ankylo",     "Tyrannosaurus", "Indominus rex", "Brachio");
Caller.F2("Gallimimus", "Indor",         "Carno",         "Spino");
```

**Attribute example — before:**

```csharp
[TestCase("Tyrannosaurus", "Rex")]
[TestCase("Velociraptor", "Raptor")]
[TestCase("Moros", "Moros")]
[TestCase("Pachycephalosaurus", "Pachy")]
```

**After (SpaceBeforeComma + align closing parenthesis):**

```csharp
[TestCase("Tyrannosaurus"     , "Rex"   )]
[TestCase("Velociraptor"      , "Raptor")]
[TestCase("Moros"             , "Moros" )]
[TestCase("Pachycephalosaurus", "Pachy" )]
```
