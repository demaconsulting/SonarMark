# Agent Quick Reference

Project-specific guidance for agents working on SonarMark - a .NET CLI tool for creating code quality reports from
SonarQube/SonarCloud analysis results.

## Standards Application (ALL Agents Must Follow)

Before performing any work, agents must read and apply the relevant standards from `.github/standards/`:

- **`csharp-language.md`** - For C# code development (literate programming, XML docs, dependency injection)
- **`csharp-testing.md`** - For C# test development (AAA pattern, naming, MSTest anti-patterns)
- **`reqstream-usage.md`** - For requirements management (traceability, semantic IDs, source filters)
- **`reviewmark-usage.md`** - For file review management (review-sets, file patterns, enforcement)
- **`software-items.md`** - For software categorization (system/subsystem/unit/OTS classification)
- **`technical-documentation.md`** - For documentation creation and maintenance (structure, Pandoc, README best practices)

Load only the standards relevant to your specific task scope and apply their
quality checks and guidelines throughout your work.

## Agent Delegation Guidelines

The default agent should handle simple, straightforward tasks directly.
Delegate to specialized agents only for specific scenarios:

- **Light development work** (small fixes, simple features) → Call @developer agent
- **Light quality checking** (linting, basic validation) → Call @quality agent
- **Formal feature implementation** (complex, multi-step) → Call the `@implementation` agent
- **Formal bug resolution** (complex debugging, systematic fixes) → Call the `@implementation` agent
- **Formal reviews** (compliance verification, detailed analysis) → Call @code-review agent
- **Template consistency** (downstream repository alignment) → Call @repo-consistency agent

## Available Specialized Agents

- **code-review** - Agent for performing formal reviews using standardized
  review processes
- **developer** - General-purpose software development agent that applies
  appropriate standards based on the work being performed
- **implementation** - Orchestrator agent that manages quality implementations
  through a formal state machine workflow
- **quality** - Quality assurance agent that grades developer work against DEMA
  Consulting standards and Continuous Compliance practices
- **repo-consistency** - Ensures SonarMark remains consistent with TemplateDotNetTool template patterns

## Quality Gate Enforcement (ALL Agents Must Verify)

Configuration files and scripts are self-documenting with their design intent and
modification policies in header comments.

1. **Linting Standards**: `./lint.sh` (Unix) or `lint.bat` (Windows) - comprehensive linting suite
2. **Build Quality**: Zero warnings (`TreatWarningsAsErrors=true`)
3. **Static Analysis**: SonarQube/CodeQL passing with no blockers
4. **Requirements Traceability**: `dotnet reqstream --enforce` passing
5. **Test Coverage**: All requirements linked to passing tests
6. **Documentation Currency**: All docs current and generated
7. **File Review Status**: All reviewable files have current reviews

## Tech Stack

- C# (latest), .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet

## Key Files

- **`requirements.yaml`** - All requirements with test linkage (enforced via `dotnet reqstream --enforce`)
- **`.editorconfig`** - Code style (file-scoped namespaces, 4-space indent, UTF-8, LF endings)
- **`.cspell.yaml`, `.markdownlint-cli2.yaml`, `.yamllint.yaml`** - Linting configs
- **`.vscode/tasks.json`** - VS Code tasks for build, test, lint, and quality checks

## Requirements (SonarMark-Specific)

- All requirements MUST be linked to tests (prefer integration tests over unit tests)
- Not all tests need to be linked to requirements (tests may exist for corner cases, design testing,
  failure-testing, etc.)
- Enforced in CI: `dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce`
- When adding features: add requirement + link to test

## Test Source Filters

Test links in `requirements.yaml` can include a source filter prefix to restrict which test results count as
evidence. This is critical for platform and framework requirements - **do not remove these filters**.

- `windows@TestName` - proves the test passed on a Windows platform
- `ubuntu@TestName` - proves the test passed on a Linux (Ubuntu) platform
- `macos@TestName` - proves the test passed on a macOS platform
- `net8.0@TestName` - proves the test passed under the .NET 8 target framework
- `net9.0@TestName` - proves the test passed under the .NET 9 target framework
- `net10.0@TestName` - proves the test passed under the .NET 10 target framework
- `dotnet8.x@TestName` - proves the self-validation test ran on a machine with .NET 8.x runtime
- `dotnet9.x@TestName` - proves the self-validation test ran on a machine with .NET 9.x runtime
- `dotnet10.x@TestName` - proves the self-validation test ran on a machine with .NET 10.x runtime

Without the source filter, a test result from any platform/framework satisfies the requirement. Adding the
filter ensures the CI evidence comes specifically from the required environment.

## Testing (SonarMark-Specific)

- **Test Naming**: `ClassName_MethodUnderTest_Scenario_ExpectedBehavior` (for requirements traceability)
- **MSTest v4**: Use `Assert.HasCount()`, `Assert.IsEmpty()`, `Assert.DoesNotContain()` (not old APIs)
- **Console Tests**: Always save/restore `Console.Out` in try/finally

## Code Style (SonarMark-Specific)

- **XML Docs**: On ALL members (public/internal/private) with spaces after `///` in summaries
- **Errors**: `ArgumentException` for parsing, `InvalidOperationException` for runtime, Write* only after success
- **No code duplication**: Extract to properties/methods

## Linting (SonarMark-Specific)

- **AI agent markdown files** (`.github/agents/*.md`): Use inline links `[text](url)` so URLs are visible in
  agent context
- **README.md**: Absolute URLs only (shipped in NuGet package)
- **Other .md**: Reference-style links `[text][ref]` with `[ref]: url` at end
- **All linters must pass locally**: markdownlint, cspell, yamllint (see `.vscode/tasks.json` or CI workflows)

## Build & Quality (Quick Reference)

```bash
# Standard build/test
dotnet build --configuration Release && dotnet test --configuration Release

# Linting
./lint.sh

# Requirements traceability
dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce
```

## Agent Report Files

Upon completion, create a report file at `.agent-logs/[agent-name]-[subject]-[unique-id].md` that includes:

- A concise summary of the work performed
- Any important decisions made and their rationale
- Follow-up items, open questions, or TODOs

Store agent logs in the `.agent-logs/` folder so they are ignored via `.gitignore` and excluded from linting and commits.
