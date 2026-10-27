# Alignator - Developer Information

This document is for developers and AI agents working **on** Alignator. If you only want to
*use* the plugin, see [README.md](README.md).

## How the project works

Alignator is a Rider/ReSharper plugin built from the official
[rider-plugin-template](https://github.com/JetBrains/rider-plugin-template) and consists of two
artifacts that are always published together with the same version:

1. **ReSharper backend (.NET, `src/dotnet/`)** — the entire feature set. It hooks into the
   ReSharper C# code formatter via `AlignArgumentsProvider`
   (extends `CSharpFormatterInfoProviderPart`) and registers 14 `IntAlignRule` alignment rules
   with alignment priority `100_000` (high, runs after standard formatting).
2. **Rider frontend (`src/rider/` + Gradle)** — a thin wrapper. `plugin.xml` declares the plugin
   identity; Gradle packages the .NET assembly into the Rider plugin zip.

### Core mechanics

- All alignments are driven by ReSharper's built-in column-alignment engine: **no Alignator rule
  takes effect unless the built-in `int_align_variables` switch (`CSharpFormatSettingsKey.INT_ALIGN_VARIABLES`)
  is enabled.** The settings page surfaces this switch at the top of the Alignator group.
- Each rule computes an alignment key consisting of an align-key group (`"Params"`, `"Func"`,
  `"ParamsAssign"`, `"Attr"`), the comma position (`ArgComma`/`ArgAfterComma`), the argument index,
  and the document offset of the enclosing block (`Helpers/NodeHelpers.cs → GetContainingBlockOffset`),
  which scopes alignment to consecutive statements of the same block. Per-call rules (argument names)
  and per-initializer rules (dictionary entries, with expressions) use the call/initializer start
  offset instead, so alignment never leaks across independent calls.
- Settings are stored in `AlignatorSettingsKey` (8 entries) with
  `[EditorConfigEntryAlias]` attributes that provide the `csharp_alignator_*` `.editorconfig` keys.
  `AlignatorSettingsSchema` builds the settings UI page with live previews.
- When a call uses named arguments, the comma alignment rules stand back for that call so comma
  alignment and argument-name alignment never pad the same lines twice.

## Where to find what

```
├── Alignator.sln                        # .NET solution (plugin + tests)
├── build.gradle.kts                     # Gradle build (IntelliJ Platform Gradle Plugin 2.x):
│                                        #   buildPlugin → zip, publishPlugin → Marketplace
├── gradle.properties                    # ⚠ plugin version (PluginVersion), SDK version (ProductVersion)
├── settings.gradle.kts                  # includes :protocol subproject (rd-gen)
├── publishPlugin.ps1                    # alternative .NET-only publish (MSBuild Pack + nuget push)
├── buildPlugin.ps1                      # .NET-only package build (MSBuild Pack)
├── settings.ps1                         # shared PowerShell helpers (MSBuild/vswhere discovery)
├── CHANGELOG.md                         # release notes source (Keep a Changelog format)
├── README.md                            # user-facing documentation
├── docs/PlanRules.md                    # workflow rules for implementation tasks
├── AGENTS.md                            # rules for AI agents working in this repo
├── output/                              # build artifacts (zip + nupkg), not committed
└── src/
    ├── dotnet/
    │   ├── Directory.Build.props        # shared MSBuild props, WaveVersion derivation
    │   ├── Plugin.props                 # ⚠ SdkVersion — keep in sync with gradle.properties
    │   ├── Alignator/
    │   │   ├── Alignator.csproj         # ReSharper build (RESHARPER define, IsPackable → nupkg)
    │   │   ├── Alignator.Rider.csproj   # Rider build (RIDER define, packs no nupkg)
    │   │   ├── IAlignatorZone.cs        # zone definition, ZoneMarker.cs — zone activator
    │   │   ├── Helpers/NodeHelpers.cs   # GetContainingBlockOffset
    │   │   └── Psi/CodeStyle/Formatting/
    │   │       ├── AlignArgumentsProvider.cs   # core: registers all 14 alignment rules
    │   │       ├── AlignPosition.cs            # DoNotChange | SpaceBeforeComma | SpaceAfterComma
    │   │       ├── AlignatorSettingsKey.cs     # settings + .editorconfig aliases
    │   │       └── AlignatorSettingsSchema.cs  # settings UI page with live preview
    │   └── Alignator.Tests/             # NUnit formatter tests
    │       ├── CodeFormatter/CSharp/AlignArguments/CSharpQuirkyFormattingTest.cs
    │       └── test/data/.../*.cs(.gold)        # input + expected output per test
    └── rider/main/resources/META-INF/plugin.xml  # Rider plugin descriptor
```

## Build, run, test

Prerequisites: .NET SDK, .NET Framework 4.7.2 targeting pack, JDK 17+, Visual Studio MSBuild
(discovered via `tools/vswhere.exe` on Windows).

```bash
./gradlew build          # full build: .NET compile + tests + zip + nupkg → output/
./gradlew buildPlugin    # Rider plugin zip only
./gradlew compileDotNet  # .NET compile only
./gradlew testDotNet     # run all formatter tests
./gradlew runIde         # run Rider sandbox with the plugin
```

Tests can also be run directly:

```bash
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj --filter "FullyQualifiedName~CSharpQuirkyFormattingTest"
```

Every formatter test needs a `.cs` input and a `.cs.gold` expected-output file under
`test/data/CodeFormatter/CSharp/AlignArguments/`. Example values must use Jurassic Park dinosaur
names (see AGENTS.md, "Jurassic Park rule").

## Versioning — where and how to increase the version

The version lives in **`gradle.properties` → `PluginVersion`**. It flows into the Rider zip
(`Alignator-<version>.zip`) and the ReSharper nupkg (`Alignator.<version>.nupkg`) via the Gradle
build. `buildPlugin.ps1`/`publishPlugin.ps1` take a `-Version` parameter instead.

When preparing a release:

1. Set `PluginVersion` in `gradle.properties` to the new SemVer version (`MAJOR.MINOR.PATCH`).
2. Add a new section in `CHANGELOG.md` (move "Unreleased" content under the new version heading).
   The Gradle build extracts the topmost changelog section into the plugin release notes.

**JetBrains Marketplace rules for versions** (see
[Semantic versioning](https://plugins.jetbrains.com/docs/marketplace/semver.html)):

- Versions must follow **SemVer** (`MAJOR.MINOR.PATCH`); SemVer checking is enabled by default for
  new plugins and can be enforced with the *SemVer Only* option on the plugin page.
- The current `9999.0.x` values are **development placeholders** from the template and must never
  be published; the first release should be something like `1.0.0`.
- Compatibility with Rider/ReSharper versions is *not* derived from the plugin version but from
  the `Wave` NuGet dependency (`253.0.0` ⇔ Rider/ReSharper 2025.3), which is generated from
  `SdkVersion` in `src/dotnet/Plugin.props`. Keep `SdkVersion` in sync with
  `ProductVersion` in `gradle.properties`.

## Publishing

Two channels, both hosted on JetBrains Marketplace:

- **Rider zip** — uploaded via the Marketplace upload API (Gradle `publishPlugin` task).
- **ReSharper nupkg** — uploaded with `dotnet nuget push` (or `nuget push`) to
  `https://plugins.jetbrains.com`, per the
  [.NET plugin upload docs](https://plugins.jetbrains.com/docs/marketplace/plugin-upload.html).

```bash
# All-in-one: runs tests, builds zip + nupkg, pushes both to Marketplace
./gradlew publishPlugin
# PublishToken must be set in gradle.properties (permanent token from your JetBrains Marketplace profile)
```

Alternatively, .NET only (uses MSBuild Pack + nuget push, changelog as release notes):

```powershell
.\publishPlugin.ps1 -Version 1.0.0 -ApiKey "<permanent token>"
```

### Checklist before the first release

The repo still contains template placeholders that JetBrains Marketplace
[approval guidelines](https://plugins.jetbrains.com/docs/marketplace/jetbrains-marketplace-approval-guidelines.html)
would reject. Before the first publish:

- [ ] Replace `PluginVersion=9999.0.0` in `gradle.properties` with a real SemVer version.
- [ ] Fill `plugin.xml`: `<version>`, `since-build`/`until-build` placeholders, and a real
      `<description>` (the placeholder "Sample description" will fail approval; the description
      becomes the Marketplace plugin page and may use basic HTML).
- [ ] Fill `src/dotnet/Plugin.props`: `Description`, `Authors`, `PackageProjectUrl`,
      `PackageLicenseUrl`/`PackageIconUrl`.
- [ ] First upload of a **new** plugin must be done **manually** on
      <https://plugins.jetbrains.com> (an upload via API/publishPlugin cannot create the plugin
      entry); subsequent versions can be pushed with `publishPlugin`.
- [ ] Configure **plugin signing** for the Rider zip — Marketplace requires plugins to be signed
      before upload (see [Plugin Signing](https://plugins.jetbrains.com/docs/marketplace/plugin-signing.html),
      and [Publishing a Plugin](https://plugins.jetbrains.com/docs/intellij/publishing-plugin.html)).
- [ ] Replace the badge/URL placeholders (`RIDER_PLUGIN_ID`, `RESHARPER_PLUGIN_ID`) in this README
      with the actual Marketplace plugin IDs after the entries exist.
- [ ] Consider renaming the NuGet package ID from `Alignator` to the `Company.Package`
      convention (e.g. `TBN.Alignator` or `ReSharperPlugin.Alignator`) — the ReSharper DevGuide
      requires a `.` in extension package IDs so the Extension Manager accepts them.

## Installing

### Released plugin (end users)

- **Rider:** Settings → Plugins → Marketplace tab → search "Alignator" → Install.
- **ReSharper (Visual Studio):** ReSharper → Extension Manager → search "Alignator" → Install.

### Local build, without Marketplace

- **Rider:** Settings → Plugins → ⚙ → *Install Plugin from Disk…* → select
  `output/Alignator-<version>.zip` (or use `./gradlew runIde` for the sandbox).
- **ReSharper:** copy the built `Alignator.<version>.nupkg` into a folder, add that folder as a
  package source in ReSharper → Options → Environment → Extension Manager, then install it via
  Extension Manager.

## Making the plugin required in a repository

JetBrains does not provide a hard "this repo requires plugin X" enforcement for Rider/ReSharper.
The recommended approach is to pin the *configuration* in the repository so every developer with
Alignator gets identical alignment, and to document the plugin as a prerequisite:

1. Commit the Alignator keys to the repo's `.editorconfig` (see
   [README.md](README.md#configuration-via-editorconfig)). Machines without the plugin simply
   ignore the `csharp_alignator_*` keys — no tooling breaks, but no alignment happens either.
2. Optionally ship the nupkg through a private company NuGet feed listed in
   ReSharper → Options → Environment → Extension Manager, so ReSharper users can install it
   without Marketplace access.
3. Optionally add a CI check (e.g. `dotnet build` + compare formatted output) that fails when
   code committed by machines without the plugin is not aligned.
