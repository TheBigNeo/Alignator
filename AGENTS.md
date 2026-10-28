# AGENTS.md — Alignator

You are a direct, critical, efficiency-focused assistant.

## Core Principles

- **Output:** Answer first. Max 5 bullets, 12 words each by default. No filler, praise, repetition, or unnecessary questions.
- **Clarity:** Minimize cognitive load. Expand only when essential or requested. Never omit critical information.
- **Reasoning:** Challenge weak assumptions. Prefer simplicity and proven approaches. Expose uncertainty and conflicts; never guess.
- **Decisions:** Give clear recommendations. Highlight material risks and irreversible consequences before suggesting approval.
- **Execution:** Read before acting. Follow conventions, minimize changes, use tools for mechanical tasks, and verify actual correctness.
- **Efficiency:** Define success, avoid unproductive loops, and report meaningful checkpoints for lengthy tasks.
- **Transparency:** Surface errors, blockers, and incomplete work. Never claim unverified success.
- DO NOT COMMIT!
- DO NOT ADD ``Co-Authored-By: Mistral ...`` TO THE COMMIT MESSAGE

Apply brevity limits to responses, not requested deliverables.

**Core principle:** Minimum reading, maximum clarity, practical value, and verified results.

---

## Table of Contents

1. [Core Principles](#core-principles)
2. [Rules](#rules)
3. [Verification Checklist](#verification-checklist)
4. [Project Overview](#project-overview)
5. [Repository Structure](#repository-structure)
6. [Implementation Details](#implementation-details)
7. [Build and Development](#build-and-development)
8. [Test Structure](#test-structure)
9. [Code Style](#code-style)
10. [Key Files](#key-files)
11. [Extending the Plugin](#extending-the-plugin)

## Rules

| #  | Rule                        | Description                                                                                                                                                                                                                  |
|----|-----------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 1  | **Think before coding**     | State assumptions out loud. Surface tradeoffs. Push back when a simpler approach exists. No silent guesses.                                                                                                                  |
| 2  | **Simplicity first**        | Minimum code that solves the stated problem. No speculative features. No abstractions for single-use code.                                                                                                                   |
| 3  | **Surgical changes**        | Touch only what the task requires. Don't "improve" adjacent code, comments, or formatting. Match existing style.                                                                                                             |
| 4  | **Goal-driven execution**   | Define success criteria up front, then loop until verified. Prefer stating the goal over dictating steps.                                                                                                                    |
| 5  | **Model limitations**       | Don't make the model do non-language work. Retries, routing, rate-limiting, arithmetic, time — write deterministic code, not prompts.                                                                                        |
| 6  | **Token efficiency**        | Hard token budget. Every loop gets a ceiling. If the same 8KB input has been re-chewed for 90 minutes, stop and step back.                                                                                                   |
| 7  | **Conflict resolution**     | Surface conflicts, don't average them. When two parts disagree, pick one visibly and explain why.                                                                                                                            |
| 8  | **Read before writing**     | Before adding code, read the nearby code. New functions that duplicate existing ones break silently through import order.                                                                                                    |
| 9  | **Test quality**            | Tests are gated by correctness, not "pass." Tie assertions to behavior, not shape.                                                                                                                                           |
| 10 | **Checkpoints**             | Long-running operations need checkpoints. Multi-step refactors commit between steps so one bad turn doesn't require rewinding six.                                                                                           |
| 11 | **Convention over novelty** | In a codebase with an established pattern, use that pattern even when yours is "better."                                                                                                                                     |
| 12 | **Visible failures**        | Fail visibly, not silently. Surface partial failure, skipped rows, truncated output, retry exhaustion.                                                                                                                       |
| 13 | **Jurassic Park rule**      | All Example Data Values use Jurassic Park dinosaur names (e.g., `Tyrannosaurus`, `Velociraptor`, `Triceratops`, `T-Rex`, `Blue`, `Echo`). Applies to test data, settings previews, doc comments, and documentation examples. |

## Verification Checklist

Before returning a task as done:

- [ ] State assumptions explicitly
- [ ] Verify no changes touched code outside the stated scope (revert or justify if yes)
- [ ] Ensure tests verify actual behavior, not just pass
- [ ] Surface any partial failure, skipped records, or truncated output in the summary

## Project Overview

**Alignator** is a JetBrains plugin for Rider and ReSharper that vertically aligns arguments in C# code.

### Purpose

Vertical alignment of arguments in constructor calls (`new Type(...)`), function invocations (`Foo(...)`), attributes (`[TestCase(...)]`), of named-argument values, of `{"Key", Value}`
dictionary-style entries in collection initializers, and of property assignments in
`with` expression initializers. The plugin pads whitespace so aligned
parts line up in columns, improving readability of dense code.

### Technical Details

| Aspect              | Value                                   |
|---------------------|-----------------------------------------|
| Plugin ID           | `com.jetbrains.rider.plugins.alignator` |
| Namespace           | `TBN.Alignator`                         |
| Target Framework    | .NET Framework 4.7.2 (`net472`)         |
| ReSharper/Rider SDK | 2025.3.0.1                              |
| Rider Platform      | 2025.3                                  |

## Repository Structure

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
│   │   │       ├── AlignArgumentsProviderBase.cs               # Shared rule builders, tree helpers, variant record, priority
│   │   │       ├── AlignPosition.cs                            # Comma modes: DoNotChange, SpaceBeforeComma, SpaceAfterComma
│   │   │       ├── FirstArgumentPosition.cs                    # First-argument modes: DoNotChange, SpaceBeforeParenthese, SpaceAfterParenthese
│   │   │       ├── AlignatorSettingsKey.cs                     # Settings keys + .editorconfig aliases
│   │   │       ├── AlignatorSettingsSchema.cs                  # Settings UI page
│   │   │       ├── AttributeArgumentsProvider.cs               # Attribute rules (ALIGN_ATTRIBUTE_ARGUMENTS*)
│   │   │       ├── CollectionInitializerArgumentsProvider.cs   # Collection initializer rules (ALIGN_COLLECTION_INITIALIZER_*)
│   │   │       ├── ConstructorArgumentsProvider.cs             # Constructor argument rules (ALIGN_CONSTRUCTOR_ARGUMENTS*)
│   │   │       ├── FunctionArgumentsProvider.cs                # Invocation argument rules (ALIGN_FUNCTION_ARGUMENTS*)
│   │   │       ├── NamedArgumentsProvider.cs                   # Named-argument rule (ALIGN_ARGUMENTS_NAMED)
│   │   │       └── WithExpressionAssignmentsProvider.cs        # With-expression rule (ALIGN_WITH_EXPRESSION_ASSIGNMENTS)
│   │   └── Alignator.Tests/       # NUnit test project
│   │       ├── Alignator.Tests.csproj
│   │       ├── TestEnvironment.cs         # Test zone setup
│   │       ├── GlobalUsings.cs
│   │       ├── CodeFormatter/CSharp/AlignArguments/
│   │       │   ├── ConstructorArgumentsFormattingTest.cs          # Constructor tests (9)
│   │       │   ├── FunctionArgumentsFormattingTest.cs             # Function tests (5)
│   │       │   ├── AttributeArgumentsFormattingTest.cs           # Attribute tests (5)
│   │       │   ├── NamedArgumentsFormattingTest.cs               # Named-argument test (1)
│   │       │   ├── CollectionInitializerFormattingTest.cs         # Collection initializer tests (7)
│   │       │   ├── WithExpressionFormattingTest.cs               # With-expression tests (4)
│   │       │   └── MixedContextsFormattingTest.cs                # Cross-context combinations (4)
│   │       ├── Data/
│   │       │   ├── TestAlignatorAttribute.cs
│   │       │   └── TestData.cs
│   │       └── test/data/CodeFormatter/CSharp/AlignArguments/
│   │           ├── TestSupport.cs      # Attribute stubs for the test data
│   │           ├── Attributes/         # Attribute data (folder named plural: 'Attribute' clashes with System.Attribute)
│   │           ├── CollectionInitializer/
│   │           ├── Constructor/
│   │           ├── Function/
│   │           ├── Mixed/              # Combinations across contexts
│   │           ├── Named/
│   │           └── WithExpression/     # One folder per context, .cs input + .cs.gold expected output each
│   └── rider/
│       └── main/resources/META-INF/plugin.xml  # Plugin descriptor
└── tools/                         # nuget.exe, vswhere.exe (Windows build helpers)
```

## Implementation Details

### Architecture

The plugin hooks into the ReSharper C# code formatter via six provider classes, one per context (`ConstructorArgumentsProvider`, `FunctionArgumentsProvider`, `AttributeArgumentsProvider`,
`NamedArgumentsProvider`, `CollectionInitializerArgumentsProvider`, `WithExpressionAssignmentsProvider`),
all deriving from `AlignArgumentsProviderBase`, which extends `CSharpFormatterInfoProviderPart`.
The platform aggregates all parts (`IImmutableEnumerable<CSharpFormatterInfoProviderPart>`); together
they register twenty-two alignment rules with alignment priority `100_000` (high, runs after standard formatting).

### Alignment Rules

| Rule                                                          | Setting                                       | Parent Node                     | Statement Type / Group                                    |
|---------------------------------------------------------------|-----------------------------------------------|---------------------------------|-----------------------------------------------------------|
| Constructor + SpaceBeforeComma                                | `ALIGN_CONSTRUCTOR_ARGUMENTS`                 | `IObjectCreationExpression`     | `IDeclarationStatement`                                   |
| Constructor + SpaceAfterComma                                 | `ALIGN_CONSTRUCTOR_ARGUMENTS`                 | `IObjectCreationExpression`     | `IDeclarationStatement`                                   |
| Constructor first argument + SpaceBeforeParenthese            | `ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST`           | `IObjectCreationExpression`     | `IDeclarationStatement`                                   |
| Constructor first argument + SpaceAfterParenthese             | `ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST`           | `IObjectCreationExpression`     | `IDeclarationStatement`                                   |
| Constructor assignment + SpaceBeforeComma                     | `ALIGN_CONSTRUCTOR_ARGUMENTS`                 | `IObjectCreationExpression`     | Assignment or field/property initializer                  |
| Constructor assignment + SpaceAfterComma                      | `ALIGN_CONSTRUCTOR_ARGUMENTS`                 | `IObjectCreationExpression`     | Assignment or field/property initializer                  |
| Constructor assignment first argument + SpaceBeforeParenthese | `ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST`           | `IObjectCreationExpression`     | Assignment or field/property initializer                  |
| Constructor assignment first argument + SpaceAfterParenthese  | `ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST`           | `IObjectCreationExpression`     | Assignment or field/property initializer                  |
| Function + SpaceBeforeComma                                   | `ALIGN_FUNCTION_ARGUMENTS`                    | `IInvocationExpression`         | `IExpressionStatement`                                    |
| Function + SpaceAfterComma                                    | `ALIGN_FUNCTION_ARGUMENTS`                    | `IInvocationExpression`         | `IExpressionStatement`                                    |
| Function first argument + SpaceBeforeParenthese               | `ALIGN_FUNCTION_ARGUMENTS_FIRST`              | `IInvocationExpression`         | `IExpressionStatement`                                    |
| Function first argument + SpaceAfterParenthese                | `ALIGN_FUNCTION_ARGUMENTS_FIRST`              | `IInvocationExpression`         | `IExpressionStatement`                                    |
| Function closing parentheses                                  | `ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN`      | `IInvocationExpression`         | `IExpressionStatement`                                    |
| Attribute + SpaceBeforeComma                                  | `ALIGN_ATTRIBUTE_ARGUMENTS`                   | `IAttribute`                    | `IAttributeSectionList`                                   |
| Attribute + SpaceAfterComma                                   | `ALIGN_ATTRIBUTE_ARGUMENTS`                   | `IAttribute`                    | `IAttributeSectionList`                                   |
| Attribute first argument + SpaceBeforeParenthese              | `ALIGN_ATTRIBUTE_ARGUMENTS_FIRST`             | `IAttribute`                    | `IAttributeSectionList`                                   |
| Attribute first argument + SpaceAfterParenthese               | `ALIGN_ATTRIBUTE_ARGUMENTS_FIRST`             | `IAttribute`                    | `IAttributeSectionList`                                   |
| Attribute closing parentheses                                 | `ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN`     | `IAttribute`                    | `IAttributeSectionList`                                   |
| Named argument values (after `name:`)                         | `ALIGN_ARGUMENTS_NAMED`                       | `IArgumentList` / `IAttribute`  | Per call/attribute (list start offset)                    |
| Dictionary entries + SpaceBeforeComma                         | `ALIGN_COLLECTION_INITIALIZER_KEY_VALUES`     | `ICollectionElementInitializer` | Per collection initializer of `IObjectCreationExpression` |
| Dictionary entries + SpaceAfterComma                          | `ALIGN_COLLECTION_INITIALIZER_KEY_VALUES`     | `ICollectionElementInitializer` | Per collection initializer of `IObjectCreationExpression` |
| Dictionary entry closing braces                               | `ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES` | `ICollectionElementInitializer` | Per collection initializer of `IObjectCreationExpression` |
| With-expression assignments                                   | `ALIGN_WITH_EXPRESSION_ASSIGNMENTS`           | `IWithInitializerAssignment`    | Per with expression                                       |

Dictionary-style entries are `ICollectionElementInitializer` nodes with at least two
`ICSharpArgument` children inside an `ICollectionInitializer` of an `IObjectCreationExpression`;
simple `{expr}` entries are never touched. Entry alignment applies only to entries that start
their own line and span a single line.

### Alignment Key Calculation

Each rule uses `IntAlignRule` with a computed alignment key. Argument rules group gaps by:

- **AlignKeyGroup**: `"Params"` for constructors, `"Func"` for functions, `"ParamsAssign"` for assignments/initializers, `"Attr"` for attributes
- **Comma position**: `ArgComma` or `ArgAfterComma`
- **Argument index** within the argument list
- **Block offset** — document offset of the enclosing block, scoping alignment to consecutive statements within the same block

The first-argument rules (`FirstArgumentPosition`: `SpaceBeforeParenthese`, `SpaceAfterParenthese`)
align the gap before/after the opening parenthesis with one shared key per block (no argument
index). Their alignment keys (`ArgBeforeOpenParen`, `ArgAfterOpenParen`) are disjoint from the
comma-rule keys, so first-argument alignment and comma alignment are independent settings that
can both be enabled at the same time.

The closing-parenthesis rules (`ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN`,
`ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN`) pad the gap before the closing parenthesis with one
shared key per block (`AttrClose`, `FuncClose`), so they combine freely with the comma settings.
The engine only groups gaps on consecutive lines, so in practice they align the closing
parentheses of consecutive single-line invocations or attributes; a closing parenthesis wrapped
onto its own line is never padded.

Per-call rules (argument names) and per-initializer rules (dictionary entries) use the argument
list / collection initializer start offset as the alignment group instead of the block offset,
and the dictionary entry rules use one shared key per initializer without an element index: the
engine aligns the k-th same-key gap of each line as one column group.

### Configuration

Twelve settings. `AlignPosition` values (arguments after the first): `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`.
`FirstArgumentPosition` values (the first argument): `DoNotChange`, `SpaceBeforeParenthese`, `SpaceAfterParenthese` —
these are independent of the `AlignPosition` settings and combine freely with them.

| Setting                                       | Type                    | Description                                                    |
|-----------------------------------------------|-------------------------|----------------------------------------------------------------|
| `ALIGN_CONSTRUCTOR_ARGUMENTS`                 | `AlignPosition`         | Aligns arguments in `new Type(...)` calls                      |
| `ALIGN_FUNCTION_ARGUMENTS`                    | `AlignPosition`         | Aligns arguments in `Foo(...)` calls                           |
| `ALIGN_ATTRIBUTE_ARGUMENTS`                   | `AlignPosition`         | Aligns arguments in `[TestCase(...)]` attributes               |
| `ALIGN_CONSTRUCTOR_ARGUMENTS_FIRST`           | `FirstArgumentPosition` | Aligns the first argument of `new Type(...)` calls             |
| `ALIGN_FUNCTION_ARGUMENTS_FIRST`              | `FirstArgumentPosition` | Aligns the first argument of `Foo(...)` calls                  |
| `ALIGN_ATTRIBUTE_ARGUMENTS_FIRST`             | `FirstArgumentPosition` | Aligns the first argument of `[TestCase(...)]` attributes      |
| `ALIGN_ATTRIBUTE_ARGUMENTS_CLOSING_PAREN`     | `bool`                  | Aligns closing parentheses of attributes                       |
| `ALIGN_FUNCTION_ARGUMENTS_CLOSING_PAREN`      | `bool`                  | Aligns closing parentheses of `Foo(...)` calls                 |
| `ALIGN_ARGUMENTS_NAMED`                       | `bool`                  | Aligns values of named arguments                               |
| `ALIGN_COLLECTION_INITIALIZER_KEY_VALUES`     | `AlignPosition`         | Aligns key-values of `{"Key", Value}` dictionary-style entries |
| `ALIGN_COLLECTION_INITIALIZER_CLOSING_BRACES` | `bool`                  | Aligns closing braces of dictionary-style entries              |
| `ALIGN_WITH_EXPRESSION_ASSIGNMENTS`           | `bool`                  | Aligns assignments in `with` expression initializers           |

**.editorconfig** configuration:

```ini
[*.cs]
csharp_alignator_align_constructor_arguments = space_after_comma
csharp_alignator_align_function_arguments = space_after_comma
csharp_alignator_align_attribute_arguments = space_after_comma
csharp_alignator_align_constructor_arguments_first = do_not_change
csharp_alignator_align_function_arguments_first = do_not_change
csharp_alignator_align_attribute_arguments_first = do_not_change
csharp_alignator_align_attribute_arguments_closing_paren = true
csharp_alignator_align_arguments_named = true
csharp_alignator_align_collection_initializer_key_values = space_after_comma
csharp_alignator_align_collection_initializer_closing_braces = true
csharp_alignator_align_with_expression_assignments = true
```

**UI Location:** Settings → Code Editing → C# → Code Style → Others → Alignator align arguments

## Build and Development

### Prerequisites

- .NET Framework 4.7.2 reference assemblies
- JDK 17 (for Gradle)
- On Windows: Visual Studio (MSBuild located via `tools/vswhere.exe`)

### Build Commands

```bash
# Full build (compiles .NET, packages plugin zip + NuGet)
./gradlew build

# Build plugin zip only
./gradlew buildPlugin

# Compile .NET only
./gradlew compileDotNet

# Run Rider with the plugin (sandbox)
./gradlew runIde
```

### Test Commands

```bash
# Run all tests via Gradle
./gradlew testDotNet

# Run tests directly with dotnet (one project at a time)
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj

# Run a specific test class
dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj --filter "FullyQualifiedName~ConstructorArgumentsFormattingTest"
```

### Lint Commands

```bash
skirnir-lint          # Builds changed projects + Roslynator analyzers
skirnir-lint --fix    # Also applies dotnet format
```

## Test Structure

Tests use the ReSharper test framework (`CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>`).

### Test Organization

Each test:

1. Has a `.cs` input file and a `.cs.gold` expected-output file under `test/data/CodeFormatter/CSharp/AlignArguments/<Context>/` (context folders: `Constructor`, `Function`, `Attributes`, `Named`, `CollectionInitializer`, `WithExpression`; `Mixed` for cross-context combinations)
2. Uses `[TestSetting]` attributes to configure `AlignatorSettingsKey` values
3. Carries `<seealso>` doc comments linking test method and test data class in both directions - clickable in the IDE
4. Calls `DoNamedTest2()` which formats the input and compares against the gold file

### Test Data

Test data stubs (`TestData.cs`) define dummy types (`StringSetting`, `IntSetting`, `Caller.F1`, etc.) so test files compile without external dependencies.

The test `.cs` data files are **compiled** (`Compile Include` in `Alignator.Tests.csproj`) even though the
formatter tests read them as text: compilation makes the `<seealso>` crefs between
the formatting test classes and the test data classes resolve in the IDE. `GlobalUsings.cs` provides
`TBN.Alignator.Tests.Data` and `TBN.Alignator.Psi.CodeStyle.Formatting` to them. The attribute names used
in the attribute test data (`Test`, `TestCase`, `TyrannosaurusTestCase`, `RaptorTest`,
`PachycephalosaurusTestCase`) are stubs in `TestSupport.cs` inside the test-data namespace - they take
precedence over NUnit's attributes there and never clash with the real test files. CS0219 is suppressed
in the test project because the test data contains unused `sortIndex` locals on purpose (they keep
alignment groups apart).

The test `.editorconfig` (`src/dotnet/Alignator.Tests/.editorconfig`) sets `indent_size = 2` for test data files and disables various ReSharper highlighting rules.

## Code Style

- **Language:** C# with `LangVersion=Latest`
- **Indentation:** 4 spaces for source code, 2 spaces for test data files
- **Namespaces:** File-scoped
- **Nullable:** Enabled where `#nullable enable` is present
- **EditorConfig:** Root `.editorconfig` enables `resharper_class_can_be_sealed_*_highlighting = warning`

## Key Files

| File                                                                                           | Purpose                                                                             |
|------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------|
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProviderBase.cs`                  | Shared rule builders, tree helpers, variant record, priority                        |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/*ArgumentsProvider.cs`                          | One provider class per context — registers that context's alignment rules           |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs`                        | Settings definitions with .editorconfig aliases                                     |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs`                     | Settings UI page with live preview                                                  |
| `src/dotnet/Alignator/Helpers/NodeHelpers.cs`                                                  | `GetContainingBlockOffset` — scopes alignment to a block                            |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/*FormattingTest.cs` | The 35 formatter tests — one test class per context + `MixedContextsFormattingTest`                                                              |
| `build.gradle.kts`                                                                             | Gradle build: compiles .NET, runs tests, packages plugin                            |
| `gradle.properties`                                                                            | Version numbers, SDK version, plugin IDs                                            |
| `src/dotnet/Plugin.props`                                                                      | `SdkVersion=2025.3.0.1` — keep in sync with `ProductVersion` in `gradle.properties` |

## Extending the Plugin

To add a new alignment mode:

1. Add a new value to the `AlignPosition` enum (arguments after the first) or the `FirstArgumentPosition` enum (the first argument)
2. Add a new method in the matching context provider's `Initialize()` following the existing pattern
3. Add a new test in the matching context formatting test class with corresponding `.cs`/`.cs.gold` test data files in that context folder
4. Update `AlignatorSettingsSchema.cs` if the new mode needs a settings UI entry
