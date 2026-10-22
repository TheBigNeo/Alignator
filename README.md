# Alignator for Rider and ReSharper

[![Rider](https://img.shields.io/jetbrains/plugin/v/RIDER_PLUGIN_ID.svg?label=Rider&colorB=0A7BBB&style=for-the-badge&logo=rider)](https://plugins.jetbrains.com/plugin/RIDER_PLUGIN_ID)
[![ReSharper](https://img.shields.io/jetbrains/plugin/v/RESHARPER_PLUGIN_ID.svg?label=ReSharper&colorB=0A7BBB&style=for-the-badge&logo=resharper)](https://plugins.jetbrains.com/plugin/RESHARPER_PLUGIN_ID)

Alignator is a code formatter plugin that vertically aligns arguments in C# constructor calls and function invocations, making your code easier to read.

## Features

- **Align arguments in constructor calls** (`new Type(...)`)
- **Align arguments in function calls** (`Foo(...)`)
- Two alignment modes:
  - `SpaceAfterComma` — pads space after each comma so arguments align in columns
  - `SpaceBeforeComma` — pads space before each comma so commas align vertically

## Settings

Settings are available in **Settings → Code Editing → C# → Code Style → Others → Alignator align arguments**.

| Setting | Description | Values |
|---|---|---|
| Align arguments in constructor | Alignment mode for `new Type(...)` calls | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` |
| Align argument in function | Alignment mode for `Foo(...)` calls | `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma` |

## .editorconfig

The plugin settings can also be configured via `.editorconfig`:

```ini
[*.cs]
# Align arguments in constructor calls
csharp_alignator_align_arguments_in_constructor = space_after_comma

# Align arguments in function calls
csharp_alignator_align_arguments_in_function = space_after_comma
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
