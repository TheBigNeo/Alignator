# Reference — Issue #2: Align Argument Names

## Goal

Add a new Alignator alignment: inside a single call whose **named arguments** are written
on separate lines, pad whitespace after the `name:` colon so all argument values start in
one column.

```csharp
// Before
Caller.F1(o1: "T-Rex",
          o2: "Raptor");

// After
Caller.F1(o1: "T-Rex",
          o2: "Raptor");
```

Expected effect (from the issue, padding after the colon, values aligned):

```csharp
AgentLogEngineMock.Setup(logEngine => logEngine.AddLokiTarget(serviceLabelValue: It.IsAny<string>(),
                                                              endpoint:          It.IsAny<string>(),
                                                              username:          It.IsAny<string>(),
                                                              ...));
```

The issue also requires:
* New **Jurassic Park themed** test data (Rule 13) — the issue's mock/Loki examples are
  *not* themed and must not be copied into test data as-is; recreate their shape with
  dinosaur values.
* **README updated as the last step.**

## Acceptance criteria

1. New formatter rule aligns named-argument values within one argument list (invocation,
   object creation, attribute) when arguments are on separate lines.
2. Controlled by a new setting, off by default (`DoNotChange`-equivalent), configurable
   via `.editorconfig` and shown on the Alignator settings page.
3. New NUnit formatter test with Jurassic Park themed `.cs` / `.cs.gold` data passes.
4. All existing formatter tests keep passing.
5. README (settings table, editorconfig section, example) updated last.

## Relevant files and symbols

| File | Key symbols / notes |
|------|---------------------|
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignArgumentsProvider.cs` | `Initialize()` registers all rules; `BuildIntAlignArgumentsRule(ArgumentAlignVariant)` is the shared comma-gap rule builder; `INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE_SpaceBeforeClosingParen()` is a hand-written `DescribeWithExternalKey<AlignatorSettingsKey, IntAlignRule>()` example with a custom alignment key — **the pattern to copy for the new rule**. `ArgumentAlignVariant` record describes variants. `AlignatorPriority = 100_000`. |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsKey.cs` | Settings storage. New entry goes here: `[EditorConfigEntryAlias("alignator_align_argument_names", ...)]` + `[SettingsEntry(false, "Align argument names")]`. |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignatorSettingsSchema.cs` | `OthersPageSchemaPart` with one `ItemFor` per setting + live preview snippet (`PreviewParseType.Member`, `IntAlignFixup`). Add an `ItemFor` for the new setting with a dinosaur preview. |
| `src/dotnet/Alignator/Psi/CodeStyle/Formatting/AlignPosition.cs` | Enum `DoNotChange, SpaceBeforeComma, SpaceAfterComma` — not used by the new rule (no comma involved). |
| `src/dotnet/Alignator/Helpers/NodeHelpers.cs` | `GetContainingBlockOffset` — scopes *cross-statement* alignment. Not needed: the new rule scopes to one argument list by its own offset. |
| `src/dotnet/Alignator.Tests/CodeFormatter/CSharp/AlignArguments/CSharpQuirkyFormattingTest.cs` | Test class: `[TestSetting]` per test + `DoNamedTest2()`. Class-level settings: `WRAP_LINES=false`, `INT_ALIGN_INVOCATIONS=true`. New test method goes here. |
| `src/dotnet/Alignator.Tests/test/data/CodeFormatter/CSharp/AlignArguments/` | One `.cs` + `.cs.gold` pair per test; 2-space indent, CRLF line endings; namespace `TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments`; `/// <seealso cref="AlignatorSettingsKey.X"/>` doc line on the test class. |
| `src/dotnet/Alignator.Tests/Data/TestData.cs` | Stub types for test files: `Caller.F1..F4(object o1, object o2 = null, ...)`, `Tyrannosaurus`, `Velociraptor`, `Triceratops`, `Brachiosaurus`, `Diet`, `DinosaurPark`. Named arguments target these `o1..o6` parameters — no new stubs needed unless a richer shape is wanted. |
| `src/dotnet/Alignator.Tests/.editorconfig` | Test-data `.editorconfig`: `indent_size = 2`, disables various ReSharper highlighting for test files. |
| `README.md` | Features list, settings table, `.editorconfig` sample, examples — all must gain the new setting (last step). |
| `CHANGELOG.md` | Keep-a-Changelog format; currently only 1.0.0. |

## Current behavior and why the change is needed

The plugin aligns gaps **around commas** across consecutive statements (keyed by
`AlignKeyGroup`, argument index, block offset). It has no rule for the gap **between a
named argument's colon and its value**, and no within-one-call alignment across lines.
Issue #2 asks for exactly that: multi-line calls with named arguments where the `name:`
labels and the values form clean columns.

## Constraints, conventions, gotchas

* **Rule 13 (Jurassic Park):** all example values in test data, settings previews, and docs
  use dinosaur names (`Tyrannosaurus`, `Velociraptor`, `Blue`, `T-Rex`, ...).
* The issue's "Actual" snippet shows an **older provider API** (`ExternalKey`,
  `StatementType`, `AlignCommaPosition`). Current code uses `SettingsKey`,
  `StatementPredicate`, `OffsetProvider`, `AlignPosition` — plan against current code.
* Built-in column-alignment engine switch `INT_ALIGN_VARIABLES` (editorconfig
  `int_align_variables`) gates all Alignator rules; tests already set
  `INT_ALIGN_INVOCATIONS = true` at class level. The new rule needs no extra switch in
  tests.
* `IntAlignOptionValue` alignment keys must be unique per alignment group; reuse the
  `$`-joined key convention, e.g. `ArgName$<argumentListStartOffset>`.
* The formatter runs at priority `100_000` after standard formatting.
* Alignment engine likely pads regardless of line breaks; the comma rules never face this
  because their gaps sit at line ends. For the new rule the gap (`:` → value) *should
  only be padded when it spans a line break* — see open questions.
* Tests: `dotnet test src/dotnet/Alignator.Tests/Alignator.Tests.csproj --filter
  "FullyQualifiedName~CSharpQuirkyFormattingTest"`. Full build/test via `./gradlew testDotNet`.
* AGENTS.md is already stale (says 6 rules; code registers 9). Out of scope — do not
  "fix" it in this task.

## Open questions / risks

1. **How to reach the named argument's colon token.** `ICSharpArgument` exposes the name /
   colon as child tokens; the colon can be found among the argument's child token nodes by
   `CSharpTokenType.COLON`. Verify the exact PSI shape at implementation time (inspect the
   tree in a failing test or via the provider's `Calculate` callback).
2. **Same-line padding.** If the rule also pads single-line calls (all named args on one
   line), that would be noisy. Decide by test observation; if it happens, restrict the
   `Calculate` result to gaps containing a line break (check the whitespace node between
   colon and value).
3. **Mixed named/positional arguments.** Only named arguments can match; positional ones
   stay untouched. The test data should include a mixed call to pin this behavior down.
4. **Scope of contexts.** Assumption: apply to invocations, object creations and
   attributes uniformly (all three existing contexts). If the comma rule for the same
   call is also on, both alignments compose (they touch different gaps).

## Assumptions (stated, not blocking)

* New setting is a plain `bool` (like `INT_ALIGN_CLOSING_PAREN_IN_ATTRIBUTE`), default
  `false`, name `INT_ALIGN_ARGUMENT_NAMES`, editorconfig alias
  `csharp_alignator_align_argument_names` — no `AlignPosition` modes needed because
  there is exactly one pad position (after the name colon).
* README is updated last, per the issue text; CHANGELOG gets an entry under a new
  Unreleased/next-version heading.
