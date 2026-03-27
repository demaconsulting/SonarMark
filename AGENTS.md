# Agent Quick Reference

Project-specific guidance for agents working on SonarMark - a .NET CLI tool for creating code quality reports from
SonarQube/SonarCloud analysis results.

## Available Specialized Agents

- **requirements** - Develops requirements and ensures test coverage linkage
- **technical-writer** - Creates accurate documentation following regulatory best practices
- **software-developer** - Writes production code and self-validation tests in literate style
- **test-developer** - Creates unit and integration tests following AAA pattern
- **code-quality** - Enforces linting, static analysis, and security standards
- **code-review** - Assists in performing formal file reviews
- **repo-consistency** - Ensures SonarMark remains consistent with TemplateDotNetTool template patterns

## Agent Selection Guide

- Fix a bug → **@software-developer**
- Add a new feature → **@requirements** → **@software-developer** → **@test-developer**
- Write a test → **@test-developer**
- Fix linting or static analysis issues → **@code-quality**
- Update documentation → **@technical-writer**
- Add or update requirements → **@requirements**
- Ensure test coverage linkage in `requirements.yaml` → **@requirements**
- Run security scanning or address CodeQL alerts → **@code-quality**
- Perform a formal file review → **@code-review**
- Propagate template changes → **@repo-consistency**

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
- **All linters must pass locally**: markdownlint, cspell, yamllint (see `.vscode/tasks.json` or CI
  workflows)

## Build & Quality (Quick Reference)

```bash
# Standard build/test
dotnet build --configuration Release && dotnet test --configuration Release

# Pre-finalization checklist (in order):
# 1. Build/test (zero warnings required)
# 2. code_review tool
# 3. codeql_checker tool
# 4. All linters (markdownlint, cspell, yamllint)
# 5. Requirements: dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce
```

## Agent Invocation Guidelines

Delegate tasks to specialized agents for better results:

- **requirements** - For: developing requirements, ensuring test coverage linkage, determining test strategy
- **technical-writer** - For: documentation updates/reviews, markdown/spell/YAML linting
- **software-developer** - For: production code features, self-validation tests, refactoring for testability
- **test-developer** - For: unit and integration tests, improving test coverage, test refactoring
- **code-quality** - For: code quality reviews, linting/static analysis issues, security verification,
  requirements traceability enforcement
- **code-review** - For: performing formal file reviews against review-set definitions
- **repo-consistency** - For: periodic reviews to ensure SonarMark follows TemplateDotNetTool patterns
