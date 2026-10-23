# AGENTS.md — Alignator

## What this repo is

**Alignator** is a JetBrains plugin (for Rider and ReSharper) that vertically aligns arguments in C# constructor calls (`new Type(...)`) and function invocations (`Foo(...)`). It is a code formatter extension that pads whitespace so arguments line up in columns, improving readability of dense argument lists.

- Plugin ID: `com.jetbrains.rider.plugins.alignator`
- Namespace: `TBN.Alignator`
- Target: .NET Framework 4.7.2 (`net472`)
- ReSharper/Rider SDK: 2025.3.0.1
- Rider platform: 2025.3

## Repository layout

```
├── Alignator.sln                  # .NET solution (3 projects)
├── build.gradle.kts               # Gradle build (IntelliJ Platform Gradle Plugin 2.x)
├── gradle.properties              # Plugin version, SDK version, build config
├── settings.gradle.kts            # Includes :protocol subproject
├── protocol/                      # rd-gen protocol definitions (Kotlin/C# codegen)
├── src/
│   ├── dotnet/
│   │   ├── Directory.Build.props  # Shared MSBuild props (LangVersion=Latest, Wave version)
│   │   ├── Plugin.props           # SdkVersion=2025.3.0.1, package metadata
│   │   ├── Alignator/             # Main plugin project (ReSharper + Rider)
│   │   │   ├── Alignator.csproj       # ReSharper plugin (RESHARPER define)
│   │   │   ├── Alignator.Rider.csproj # Rider plugin (RIDER define)
│   │   │   ├── IAlignatorZone.cs      # Zone definition (requires CSharp, Psi, CodeEditing)
│   │   │   ├── ZoneMarker.cs          # Zone activator/marker
│   │   │   ├── Helpers/
│   │   │   │   └── NodeHelpers.cs     # GetContainingBlockOffset extension
│   │   │   └── Psi/CodeStyle/Formatting/
│   │   │       ├── AlignArgumentsProvider.cs  # Core: registers 6 alignment rules
│   │   │       ├── AlignPosition.cs           # Enum: DoNotChange, SpaceBeforeComma, SpaceAfterComma
│   │   │       ├── AlignatorSettingsKey.cs    # Settings keys + .editorconfig aliases
│   │   │       └── AlignatorSettingsSchema.cs # Settings UI page ("Alignator align arguments")
│   │   └── Alignator.Tests/       # NUnit test project
│   │       ├── Alignator.Tests.csproj
│   │       ├── TestEnvironment.cs         # Test zone setup
│   │       ├── GlobalUsings.cs
│   │       ├── CodeFormatter/CSharp/AlignArguments/
│   │       │   └── CSharpQuirkyFormattingTest.cs  # 6 formatter tests
│   │       ├── Data/
│   │       │   ├── TestAlignatorAttribute.cs      # Test aspect (net451 target)
│   │       │   └── TestData.cs                    # Stub types for test data
│   │       └── test/data/CodeFormatter/CSharp/AlignArguments/
│   │           ├── *.cs          # Input test files
│   │           └── *.cs.gold     # Expected output after formatting
│   └── rider/
│       └── main/resources/META-INF/plugin.xml  # Plugin descriptor
└── tools/                         # nuget.exe, vswhere.exe (Windows build helpers)
```

## How it works

The plugin hooks into the ReSharper C# code formatter via `AlignArgumentsProvider`, which extends `CSharpFormatterInfoProviderPart`. It registers six alignment rules:

| Rule | Setting | Parent node | Statement type |
|------|---------|-------------|----------------|
| Constructor + SpaceBeforeComma | `INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR` | `IObjectCreationExpression` | `IDeclarationStatement` |
| Constructor + SpaceAfterComma | `INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR` | `IObjectCreationExpression` | `IDeclarationStatement` |
| Constructor assignment + SpaceBeforeComma | `INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR` | `IObjectCreationExpression` | assignment statement (field/property assignment) or field/property initializer |
| Constructor assignment + SpaceAfterComma | `INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR` | `IObjectCreationExpression` | assignment statement (field/property assignment) or field/property initializer |
| Function + SpaceBeforeComma | `INT_ALIGN_ARGUMENTS_IN_FUNCTION` | `IInvocationExpression` | `IExpressionStatement` |
| Function + SpaceAfterComma | `INT_ALIGN_ARGUMENTS_IN_FUNCTION` | `IInvocationExpression` | `IExpressionStatement` |

Each rule uses `IntAlignRule` with a computed alignment key that groups arguments by:
- **AlignKeyGroup** (`"Params"` for constructors, `"Func"` for functions, `"ParamsAssign"` for assignments/initializers)
- **Comma position** (`ArgComma` or `ArgAfterComma`)
- **Argument index** within the argument list
- **Block offset** — the document offset of the enclosing block, so alignment is scoped to consecutive statements within the same block

The alignment priority is `100_000` (high, so it runs after standard formatting).

### Settings

Two settings, each with three values (`DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`):

- `INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR` — aligns arguments in `new Type(...)` calls
- `INT_ALIGN_ARGUMENTS_IN_FUNCTION` — aligns arguments in `Foo(...)` calls

Also configurable via `.editorconfig`:
```ini
[*.cs]
csharp_alignator_align_arguments_in_constructor = space_after_comma
csharp_alignator_align_arguments_in_function = space_after_comma
```

Settings UI lives under **Settings → Code Editing → C# → Code Style → Others → Alignator align arguments**.

## Build and test

### Prerequisites
- .NET Framework 4.7.2 reference assemblies
- JDK 17 (for Gradle)
- On Windows: Visual Studio (MSBuild located via `tools/vswhere.exe`)

### Build
```bash
# Full build (Gradle: compiles .NET, packages plugin zip + NuGet)
./gradlew build

# Build plugin zip only
./gradlew buildPlugin

# Compile .NET only
./gradlew compileDotNet

# Run Rider with the plugin (sandbox)
./gradlew runIde
```

### Test
```bash
# Run all tests via Gradle
./gradlew testDotNet

# Run tests directly with dotnet (one project at a time)
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj

# Run a specific test class
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj --filter "FullyQualifiedName~CSharpQuirkyFormattingTest"
```

### Lint
```bash
skirnir-lint          # Builds changed projects + Roslynator analyzers
skirnir-lint --fix    # Also applies dotnet format
```

## Test structure

Tests use the ReSharper test framework (`CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>`). Each test:

1. Has a `.cs` input file and a `.cs.gold` expected-output file under `test/data/CodeFormatter/CSharp/AlignArguments/`
2. Uses `[TestSetting]` attributes to configure `AlignatorSettingsKey` values
3. Calls `DoNamedTest2()` which formats the input and compares against the gold file

Test data stubs (`TestData.cs`) define dummy types (`StringSetting`, `IntSetting`, `Caller.F1`, etc.) so test files compile without external dependencies.

The test `.editorconfig` (`src/dotnet/Alignator.Tests/.editorconfig`) sets `indent_size = 2` for test data files and disables various ReSharper highlighting rules.

## Code style

- C# with `LangVersion=Latest`
- 4-space indentation for source code (2-space for test data files)
- File-scoped namespaces
- Nullable reference types enabled where `#nullable enable` is present
- The root `.editorconfig` enables `resharper_class_can_be_sealed_*_highlighting = warning`

## Key files to know

| File | Purpose |
|------|---------|
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProvider.cs` | Core formatter logic — registers all 6 alignment rules |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs` | Settings definitions with .editorconfig aliases |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs` | Settings UI page with live preview |
| `src/dotnet/Alignator/Helpers/NodeHelpers.cs` | `GetContainingBlockOffset` — scopes alignment to a block |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/CSharpQuirkyFormattingTest.cs` | All 6 formatter tests |
| `build.gradle.kts` | Gradle build: compiles .NET, runs tests, packages plugin |
| `gradle.properties` | Version numbers, SDK version, plugin IDs |
| `src/dotnet/Plugin.props` | `SdkVersion=2025.3.0.1` — keep in sync with `ProductVersion` in `gradle.properties` |

## Adding a new alignment mode

1. Add a new value to `AlignPosition` enum
2. Add a new method in `AlignArgumentsProvider.Initialize()` following the existing pattern
3. Add a new test in `CSharpQuirkyFormattingTest.cs` with corresponding `.cs`/`.cs.gold` test data files
4. Update `AlignatorSettingsSchema.cs` if the new mode needs a settings UI entry
