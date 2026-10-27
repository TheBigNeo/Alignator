# DEVELOPERS.md — Alignator Internals

For developers and AI agents working **on** Alignator. User-facing docs: [README.md](README.md).

## How it works

#### Architecture

- Two artifacts, always published together with the same version:
    - The Rider plugin zip (frontend wrapper, `src/rider/` + Gradle)
    - The ReSharper NuGet package (all logic, `net472`, `src/dotnet/`).

#### Alignment rules

- All alignment logic lives in [AlignArgumentsProvider.cs](src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProvider.cs):
  extends `CSharpFormatterInfoProviderPart`, registers 14 `IntAlignRule` rules with priority `100_000` (runs after standard formatting).
- Rules require the built-in `int_align_variables` switch (`CSharpFormatSettingsKey.INT_ALIGN_VARIABLES`) — no Alignator alignment without it.
  The settings page shows this switch at the top of the Alignator group.
- Named-argument alignment takes precedence: comma rules stand back for calls that use named arguments, so no line is padded twice.

#### Alignment keys

- Alignment key = align-key group (`"Params"`, `"Func"`, `"ParamsAssign"`, `"Attr"`) + comma position + argument index + block offset ([NodeHelpers.cs](src/dotnet/Alignator/Helpers/NodeHelpers.cs) `GetContainingBlockOffset`) — alignment never crosses block boundaries.
- Per-call rules (named arguments) and per-initializer rules (dictionary entries, with expressions) group by call/initializer offset instead.

#### Settings

- [AlignatorSettingsKey.cs](src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs) — 8 entries with `[EditorConfigEntryAlias]` → `csharp_alignator_*` keys.
- UI: [AlignatorSettingsSchema.cs](src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs) ("Alignator align arguments" page part, live previews).

## Where to find what

| Topic                                         | Where                                                                                                                         |
|-----------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------|
| .NET solution (plugin + tests)                | [Alignator.sln](Alignator.sln)                                                                                                |
| Gradle build (`buildPlugin`, `publishPlugin`) | [build.gradle.kts](build.gradle.kts)                                                                                          |
| Plugin version, SDK version                   | [gradle.properties](gradle.properties)                                                                                        |
| `SdkVersion` (sync with `ProductVersion`)     | [src/dotnet/Plugin.props](src/dotnet/Plugin.props)                                                                            |
| Rider plugin descriptor                       | [src/rider/main/resources/META-INF/plugin.xml](src/rider/main/resources/META-INF/plugin.xml)                                  |
| Release notes source                          | [CHANGELOG.md](CHANGELOG.md)                                                                                                  |
| .NET-only pack/publish via MSBuild            | [publishPlugin.ps1](publishPlugin.ps1), [buildPlugin.ps1](buildPlugin.ps1)                                                    |
| Workflow rules for implementation tasks       | [docs/PlanRules.md](docs/PlanRules.md)                                                                                        |
| AI agent rules (incl. dinosaur names)         | [AGENTS.md](AGENTS.md)                                                                                                        |
| Formatter tests                               | [CSharpQuirkyFormattingTest.cs](src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/CSharpQuirkyFormattingTest.cs) |
| Test data (`.cs` / `.cs.gold` pairs)          | [test/data](src/dotnet/Alignator.Tests/test/data/CodeFormatter/CSharp/AlignArguments/)                                        |

## Build and test

```bash
./gradlew build          # compile + test + zip + nupkg → output/
./gradlew buildPlugin    # zip only
./gradlew compileDotNet  # .NET only
./gradlew testDotNet     # formatter tests
./gradlew runIde         # Rider sandbox with the plugin
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj
```

- Prerequisites: .NET SDK, net472 targeting pack, JDK 17+, Visual Studio MSBuild (found via `tools/vswhere.exe`).
- New test = `.cs` input + `.cs.gold` expected output under `test/data/...` + a `DoNamedTest2()` method.

## Versioning

Steps to prepare a new version:

1. Bump `PluginVersion` in [gradle.properties](gradle.properties) — it flows into the zip and the nupkg (the PowerShell scripts take `-Version` instead).
2. Add a section for the release in [CHANGELOG.md](CHANGELOG.md) — the build extracts the topmost section as release notes.

Rules to know:

- Marketplace checks SemVer (`MAJOR.MINOR.PATCH`, on by default for new plugins): <https://plugins.jetbrains.com/docs/marketplace/semver.html>
- `9999.0.x` is the template dev placeholder — never publish it.
- IDE compatibility comes from the `Wave` NuGet dependency (`253.0.0` ⇔ Rider/ReSharper 2025.3), derived from `SdkVersion` in [Plugin.props](src/dotnet/Plugin.props).

## Publish

1. Set `PublishToken` in [gradle.properties](gradle.properties) — a permanent token from your JetBrains Marketplace profile.
2. First release only: upload the plugin manually on <https://plugins.jetbrains.com> (`publishPlugin` only works for updates) and replace the template placeholders (checklist below).
3. Run `./gradlew publishPlugin` — runs tests, builds both artifacts, pushes both:
   zip via the Marketplace upload API, nupkg via `dotnet nuget push` ([.NET upload docs](https://plugins.jetbrains.com/docs/marketplace/plugin-upload.html)).
4. Alternative, .NET only: `.\publishPlugin.ps1 -Version 1.0.0 -ApiKey <token>`.
5. Plugins must be signed before upload: <https://plugins.jetbrains.com/docs/marketplace/plugin-signing.html>

Placeholder checklist for the first release:

- [plugin.xml](src/rider/main/resources/META-INF/plugin.xml): `<version>`, `since-build`/`until-build`, real `<description>` (Marketplace approval checks this).
- [Plugin.props](src/dotnet/Plugin.props): `Description`, `Authors`, `PackageProjectUrl`, `PackageLicenseUrl`, `PackageIconUrl`.
- README badge IDs (`RIDER_PLUGIN_ID`, `RESHARPER_PLUGIN_ID`).
- Consider a `Company.Package` package ID (e.g. `ReSharperPlugin.Alignator`) — the ReSharper DevGuide requires a `.` in extension package IDs.

## Install

1. Rider (released): **File | Settings | Plugins** → Marketplace → search "Alignator".
2. ReSharper (released): **ReSharper | Extension Manager** → search "Alignator".
3. Rider (local build): **File | Settings | Plugins** → ⚙ → *Install Plugin from Disk…* → `output/Alignator-<version>.zip`.
4. ReSharper (local build): add the `output` folder as a package source in **ReSharper | Options | Environment | Extension Manager**, then install via Extension Manager.
5. Dev sandbox: `./gradlew runIde`.

## Requiring the plugin in a repository

- Rider/ReSharper have no hard "this repo requires plugin X" enforcement.
- Commit the Alignator keys to the repo's `.editorconfig` (see [README.md](README.md#configuration-via-editorconfig)) — machines without the plugin simply ignore them.
- Optionally host the nupkg on a private NuGet feed listed in **ReSharper | Options | Environment | Extension Manager**.
- Optionally verify alignment in CI (e.g. compare formatted output).
