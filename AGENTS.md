# Agent Quick Reference

Project-specific guidance for agents working on SonarMark - a .NET CLI tool for creating code quality reports from
SonarQube/SonarCloud analysis results.

## Available Specialized Agents

- **Requirements Agent** - Develops requirements and ensures test coverage linkage
- **Technical Writer** - Creates accurate documentation following regulatory best practices
- **Software Developer** - Writes production code and self-validation tests in literate style
- **Test Developer** - Creates unit and integration tests following AAA pattern
- **Code Quality Agent** - Enforces linting, static analysis, and security standards
- **Repo Consistency Agent** - Ensures SonarMark remains consistent with TemplateDotNetTool template patterns

## Tech Stack

- C# 12, .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet

## Key Files

- **`requirements.yaml`** - All requirements with test linkage (enforced via `dotnet reqstream --enforce`)
- **`.editorconfig`** - Code style (file-scoped namespaces, 4-space indent, UTF-8+BOM, LF endings)
- **`.cspell.json`, `.markdownlint.json`, `.yamllint.yaml`** - Linting configs
- **`.vscode/tasks.json`** - VS Code tasks for build, test, lint, and quality checks

## Requirements (SonarMark-Specific)

- All requirements MUST be linked to tests (prefer integration tests over unit tests)
- Not all tests need to be linked to requirements (tests may exist for corner cases, design testing,
  failure-testing, etc.)
- Enforced in CI: `dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce`
- When adding features: add requirement + link to test

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

- **requirements-agent** - For: developing requirements, ensuring test coverage linkage, determining test strategy
- **technical-writer** - For: documentation updates/reviews, markdown/spell/YAML linting
- **software-developer** - For: production code features, self-validation tests, refactoring for testability
- **test-developer** - For: unit and integration tests, improving test coverage, test refactoring
- **code-quality-agent** - For: code quality reviews, linting/static analysis issues, security verification,
  requirements traceability enforcement
- **repo-consistency-agent** - For: periodic reviews to ensure SonarMark follows TemplateDotNetTool patterns
