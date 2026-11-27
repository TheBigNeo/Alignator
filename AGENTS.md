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
- DO NOT TOUCH CHANGELOG.md

Apply brevity limits to responses, not requested deliverables.

**Core principle:** Minimum reading, maximum clarity, practical value, and verified results.

---

## Table of Contents

1. [Core Principles](#core-principles)
2. [Rules](#rules)
3. [Verification Checklist](#verification-checklist)
4. [Project Overview](#project-overview)
5. [Implementation Details](#implementation-details)
6. [Build and Development](#build-and-development)
7. [Test Structure](#test-structure)
8. [Code Style](#code-style)
9. [Key Files](#key-files)
10. [Extending the Plugin](#extending-the-plugin)

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
dictionary-style entries in collection initializers, of object initializers inside
collections, and of property assignments in `with` expression initializers. The plugin pads whitespace so aligned
parts line up in columns, improving readability of dense code.

### Technical Details

| Aspect              | Value                                   |
|---------------------|-----------------------------------------|
| Plugin ID           | `com.jetbrains.rider.plugins.alignator` |
| Namespace           | `TBN.Alignator`                         |
| Target Framework    | .NET Framework 4.7.2 (`net472`)         |
| ReSharper/Rider SDK | 2025.3.0.1                              |
| Rider Platform      | 2025.3                                  |

## Implementation Details

### Architecture

The plugin hooks into the ReSharper C# code formatter via six provider classes, one per context (`ConstructorArgumentsProvider`, `FunctionArgumentsProvider`, `AttributeArgumentsProvider`,
`NamedArgumentsProvider`, `CollectionInitializerArgumentsProvider`, `ObjectInitializerArgumentsProvider`,
`WithExpressionAssignmentsProvider`), all deriving from `AlignArgumentsProviderBase`, which extends `CSharpFormatterInfoProviderPart`.
The platform aggregates all parts (`IImmutableEnumerable<CSharpFormatterInfoProviderPart>`); together
they register twenty-eight alignment rules with alignment priority `100_000` (high, runs after standard formatting).

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
| Object initializer opening braces + SpaceBeforeBrace          | `ALIGN_OBJECT_INITIALIZER_OPENING_BRACES`     | `IObjectCreationExpression`     | Per collection expression / initializer                   |
| Object initializer opening braces + SpaceAfterBrace           | `ALIGN_OBJECT_INITIALIZER_OPENING_BRACES`     | `IObjectInitializer`            | Per collection expression / initializer                   |
| Object initializer assignments + BeforeEq                    | `ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS`        | `IMemberInitializer`             | Per collection expression / initializer                   |
| Object initializer assignments + AfterComma                  | `ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS`        | `IObjectInitializer`            | Per collection expression / initializer                   |
| Object initializer closing braces                             | `ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES`     | `IObjectInitializer`            | Per collection expression / initializer                   |
| With-expression assignments                                   | `ALIGN_WITH_EXPRESSION_ASSIGNMENTS`           | `IWithInitializerAssignment`    | Per with expression                                       |

Dictionary-style entries are `ICollectionElementInitializer` nodes with at least two
`ICSharpArgument` children inside an `ICollectionInitializer` of an `IObjectCreationExpression`;
simple `{expr}` entries are never touched. Entry alignment applies only to entries that start
their own line and span a single line.

Object initializer alignment targets `IObjectCreationExpression` entries (including target-typed `new()`) with a
non-empty `IObjectInitializer` that are direct entries of one collection - a collection expression (`[ .. ]`, wrapped
in an `IExpressionElement`) or a collection initializer (`new List<T> { .. }`, wrapped in a braceless
`ICollectionElementInitializer`) - and uses the collection's start offset as the alignment group: entries of
different collections never align. Like dictionary entries, qualified entries start their own line, span a single
line, and the engine only groups gaps on consecutive lines, so a multi-line entry splits the group.

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
The object initializer rules do the same per enclosing collection with the disjoint key stems
`ObjInit` + `BeforeOpenBrace` / `AfterOpenBrace` / `BeforeEq` / `AfterComma` / `BeforeCloseBrace`,
so the three settings combine freely with each other and with all other settings.
Named-argument alignment takes precedence: the comma and first-argument rules stand back for argument lists
and attribute lists that use named arguments (including attribute Name = property assignments),
so no line is padded twice.

### Configuration

Fifteen settings. `AlignPosition` values (arguments after the first): `DoNotChange`, `SpaceBeforeComma`, `SpaceAfterComma`.
`FirstArgumentPosition` values (the first argument): `DoNotChange`, `SpaceBeforeParenthese`, `SpaceAfterParenthese` —
these are independent of the `AlignPosition` settings and combine freely with them.
`OpeningBracePosition` values (object initializer opening braces): `DoNotChange`, `SpaceBeforeBrace`, `SpaceAfterBrace`.

All Alignator rules additionally require the built-in `int_align_invocations` switch
(`CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS`, default off) - no Alignator alignment
takes effect without it. The settings page shows this switch at the top of the Alignator
group; it is the same value as in ReSharper's own formatting settings.

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
| `ALIGN_OBJECT_INITIALIZER_OPENING_BRACES`    | `OpeningBracePosition`  | Aligns opening braces in object initializer entries            |
| `ALIGN_OBJECT_INITIALIZER_ASSIGNMENTS`       | `bool`                  | Aligns assignments in object initializer entries               |
| `ALIGN_OBJECT_INITIALIZER_CLOSING_BRACES`    | `bool`                  | Aligns closing braces in object initializer entries            |
| `ALIGN_WITH_EXPRESSION_ASSIGNMENTS`           | `bool`                  | Aligns assignments in `with` expression initializers           |

**.editorconfig** configuration:

```ini
[*.cs]
# Built-in switch: required for every Alignator alignment
int_align_invocations = true

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
csharp_alignator_align_object_initializer_opening_braces = space_before_brace
csharp_alignator_align_object_initializer_assignments = true
csharp_alignator_align_object_initializer_closing_braces = true
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

## Test Structure

Tests use the ReSharper test framework (`CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>`).

### Test Organization

Each test:

1. Has a `.cs` input file and a `.cs.gold` expected-output file under `test/data/CodeFormatter/CSharp/AlignArguments/<Context>/` (context folders: `Constructor`, `Function`, `Attributes`, `Named`, `CollectionInitializer`, `ObjectInitializer`, `WithExpression`; `Mixed` for cross-context combinations). The attribute context folder is named `Attributes` (plural) because a folder named `Attribute` would clash with `System.Attribute`
2. Uses `[TestSetting]` attributes to configure `AlignatorSettingsKey` values
3. Carries `<seealso>` doc comments linking test method and test data class in both directions - clickable in the IDE
4. Calls `DoNamedTest2()` which formats the input and compares against the gold file

### Test Data

Test data stubs (`TestData.cs`) define dummy types (`StringSetting`, `IntSetting`, `Caller.FeedDinosaurs`, etc.) so test files compile without external dependencies.

The test `.cs` data files are **compiled** (`Compile Include` in `Alignator.Tests.csproj`) even though the
formatter tests read them as text: compilation makes the `<seealso>` crefs between
the formatting test classes and the test data classes resolve in the IDE. `GlobalUsings.cs` provides
`TBN.Alignator.Tests.Data` and `TBN.Alignator.Psi.CodeStyle.Formatting` to them. The attribute names used
in the attribute test data (`Test`, `TestCase`, `TyrannosaurusTestCase`, `RaptorTest`,
`PachycephalosaurusTestCase`) are stubs in `TestData.cs` inside the test-data namespace - they take
precedence over NUnit's attributes there and never clash with the real test files. CS0219 is suppressed
in the test project because the test data contains unused `sortIndex` locals on purpose (they keep
alignment groups apart).

The test `.editorconfig` (`src/dotnet/Alignator.Tests/.editorconfig`) sets `indent_size = 2` for test data files and disables various ReSharper highlighting rules.

## Code Style

- **Language:** C# with `LangVersion=Latest`
- **Indentation:** 4 spaces for source code, 2 spaces for test data files
- **Comments:** No hard 120-character limit. Wrap comment lines at intelligent breaks that follow the sentence -
  one sentence, clause, or phrase per line, like Markdown prose. A hard cut at a fixed column in the middle of a
  clause (or a compound like "the / whitespace gap") is hard to read; break after a sentence, between clauses,
  or before a parenthetical or example instead, even when lines then differ in length.
- **Namespaces:** File-scoped
- **Nullable:** Enabled where `#nullable enable` is present
- **EditorConfig:** Root `.editorconfig` enables `resharper_class_can_be_sealed_*_highlighting = warning`

## Key Files

| File                                                                                | Purpose                                                                                                         |
|-------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProviderBase.cs`       | Shared rule builders, tree helpers, variant record, priority                                                    |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/*ArgumentsProvider.cs`               | One provider class per context — registers that context's alignment rules                                       |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs`             | Settings definitions with .editorconfig aliases                                                                 |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs`          | Settings UI page with live preview                                                                              |
| `src/dotnet/Alignator/Helpers/NodeHelpers.cs`                                       | `GetContainingBlockOffset` — scopes alignment to a block                                                        |
| `src/dotnet/Alignator/Alignator.csproj`, `Alignator.Rider.csproj`               | Same sources, compiled with the `RESHARPER` and `RIDER` defines respectively |
| `protocol/`                                                                     | rd-gen protocol definitions (Kotlin/C# codegen), included as the `:protocol` subproject |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/*FormattingTest.cs` | The 60 formatter tests + 1 settings-coverage guard — one test class per context + `MixedContextsFormattingTest` |
| `build.gradle.kts`                                                                  | Gradle build: compiles .NET, runs tests, packages plugin                                                        |
| `gradle.properties`                                                                 | Version numbers, SDK version, plugin IDs                                                                        |
| `src/dotnet/Plugin.props`                                                           | `SdkVersion=2025.3.0.1` — keep in sync with `ProductVersion` in `gradle.properties`                             |

## Extending the Plugin

To add a new alignment mode:

1. Add a new value to the `AlignPosition` enum (arguments after the first), the `FirstArgumentPosition` enum (the first argument)
   or the `OpeningBracePosition` enum (object initializer opening braces)
2. Add a new method in the matching context provider's `Initialize()` following the existing pattern
3. Add a new test in the matching context formatting test class with corresponding `.cs`/`.cs.gold` test data files in that context folder
4. Update `AlignatorSettingsSchema.cs` if the new mode needs a settings UI entry
