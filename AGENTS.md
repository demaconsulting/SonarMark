# Agent Quick Reference

Project-specific guidance for agents working on SonarMark - a .NET CLI tool for creating code quality reports from
SonarQube/SonarCloud analysis results.

## Tech Stack

- C# 12, .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet

## Key Files

- **`requirements.yaml`** - All requirements with test linkage (enforced via `dotnet reqstream --enforce`)
- **`.editorconfig`** - Code style (file-scoped namespaces, 4-space indent, UTF-8+BOM, LF endings)
- **`.cspell.json`, `.markdownlint.json`, `.yamllint.yaml`** - Linting configs
- **`.vscode/tasks.json`** - VS Code tasks for build, test, lint, and quality checks

## Requirements (SonarMark-Specific)

- Link ALL requirements to tests (prefer integration tests over unit tests)
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

- **README.md**: Absolute URLs only (shipped in NuGet package)
- **Other .md**: Reference-style links `[text][ref]` with `[ref]: url` at end
- **All linters must pass locally**: markdownlint, cspell, yamllint (see `.vscode/tasks.json` or CI workflows)

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

## Custom Agents

Delegate tasks to specialized agents for better results:

- **documentation-writer** - Invoke for: documentation updates/reviews, requirements.yaml changes,
  markdown/spell/YAML linting
- **project-maintainer** - Invoke for: dependency updates, CI/CD maintenance, releases, requirements
  traceability enforcement
- **software-quality-enforcer** - Invoke for: code quality reviews, test coverage verification (>80%),
  static analysis, zero-warning builds, requirements test quality
