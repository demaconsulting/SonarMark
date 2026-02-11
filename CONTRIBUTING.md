# Contributing to SonarMark

Thank you for your interest in contributing to SonarMark! We welcome contributions from the community and appreciate
your help in making this project better.

## Code of Conduct

This project adheres to a [Code of Conduct][code-of-conduct]. By participating, you are expected to uphold this code.
Please report unacceptable behavior through GitHub.

## How to Contribute

### Reporting Bugs

If you find a bug, please create an issue on GitHub with the following information:

- **Description**: Clear description of the bug
- **Steps to Reproduce**: Detailed steps to reproduce the issue
- **Expected Behavior**: What you expected to happen
- **Actual Behavior**: What actually happened
- **Environment**: Operating system, .NET version, SonarMark version
- **Logs**: Any relevant error messages or logs

### Suggesting Features

We welcome feature suggestions! Please create an issue on GitHub with:

- **Feature Description**: Clear description of the proposed feature
- **Use Case**: Why this feature would be useful
- **Proposed Solution**: Your ideas on how to implement it (optional)
- **Alternatives**: Any alternative solutions you've considered (optional)

### Submitting Pull Requests

We follow a standard GitHub workflow for contributions:

1. **Fork** the repository
2. **Clone** your fork locally
3. **Create a branch** for your changes (`git checkout -b feature/my-feature`)
4. **Make your changes** following our coding standards
5. **Test your changes** thoroughly
6. **Commit your changes** with clear commit messages
7. **Push** to your fork
8. **Create a Pull Request** to the main repository

## Development Setup

### Prerequisites

- [.NET SDK][dotnet-download] 8.0, 9.0, or 10.0
- Git
- A code editor (Visual Studio, VS Code, or Rider recommended)

### Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/demaconsulting/SonarMark.git
   cd SonarMark
   ```

2. Restore dependencies:

   ```bash
   dotnet tool restore
   dotnet restore
   ```

3. Build the project:

   ```bash
   dotnet build --configuration Release
   ```

4. Run tests:

   ```bash
   dotnet test --configuration Release
   ```

## Coding Standards

### General Guidelines

- Follow the [C# Coding Conventions][csharp-conventions]
- Use clear, descriptive names for variables, methods, and classes
- Write XML documentation comments for all public, internal, and private members
- Keep methods focused and single-purpose
- Write tests for new functionality

### Code Style

This project enforces code style through `.editorconfig`. Key requirements:

- **Indentation**: 4 spaces for C#, 2 spaces for YAML/JSON/XML
- **Line Endings**: LF (Unix-style)
- **Encoding**: UTF-8 with BOM
- **Namespaces**: Use file-scoped namespace declarations
- **Braces**: Required for all control statements
- **Naming Conventions**:
  - Interfaces: `IInterfaceName`
  - Classes/Structs/Enums: `PascalCase`
  - Methods/Properties: `PascalCase`
  - Parameters/Local Variables: `camelCase`

### XML Documentation

All members require XML documentation with proper indentation:

```csharp
/// <summary>
///     Brief description of what this does.
/// </summary>
/// <param name="parameter">Description of the parameter.</param>
/// <returns>Description of the return value.</returns>
public int ExampleMethod(string parameter)
{
    // Implementation
}
```

Note the spaces after `///` for proper indentation in summary blocks.

## Testing Guidelines

### Test Framework

We use MSTest v4 for unit and integration tests.

### Test Naming Convention

Use the pattern: `ClassName_MethodUnderTest_Scenario_ExpectedBehavior`

Examples:

- `Program_Main_NoArguments_ReturnsSuccess`
- `Context_Create_WithInvalidFlag_ThrowsArgumentException`
- `SonarQubeClient_GetQualityResult_ValidProject_ReturnsResult`

### Writing Tests

- Write tests that are clear and focused
- Use modern MSTest v4 assertions:
  - `Assert.HasCount(collection, expectedCount)`
  - `Assert.IsEmpty(collection)`
  - `Assert.DoesNotContain(item, collection)`
- Always clean up resources (use `try/finally` for console redirection)
- Link tests to requirements in `requirements.yaml` when applicable

### Running Tests

```bash
# Run all tests
dotnet test --configuration Release

# Run specific test
dotnet test --filter "FullyQualifiedName~YourTestName"

# Run with coverage
dotnet test --collect "XPlat Code Coverage"
```

### Test Execution Strategy

SonarMark uses a multi-stage test execution strategy to validate functionality across different contexts:

#### Local Testing (Unit Tests)

Unit tests run locally and provide rapid feedback during development:

- **Scope**: Core functionality, argument parsing, report generation logic
- **Execution**: `dotnet test --configuration Release`
- **Requirements Coverage**: Validates 19 of 29 requirements locally

#### CI Integration Testing (Self-Validation Tests)

Self-validation tests execute in CI and validate the complete packaged tool:

- **Scope**: End-to-end validation against real SonarCloud APIs
- **Execution**: Runs via `sonarmark --validate` in the integration-test job
- **Requirements Coverage**: Validates requirements SONAR-003, SONAR-004, SONAR-005, RPT-001, VAL-001

#### CI Platform Testing (Matrix Tests)

Platform-specific tests execute across multiple OS and .NET runtime combinations:

- **Scope**: Windows/Linux compatibility, .NET 8/9/10 support
- **Execution**: Runs in the integration-test job matrix
- **Requirements Coverage**: Validates requirements PLT-001 through PLT-005

#### Requirements Traceability

Full requirements traceability is validated in CI using ReqStream:

```bash
dotnet reqstream --requirements requirements.yaml --tests "test-results/**/*.trx" --enforce
```

**Note**: When running `dotnet reqstream --enforce` locally, you may see 10 unsatisfied requirements. This is expected
because self-validation and platform-specific tests only execute in the CI pipeline. Full validation requires the
complete CI workflow.

## Documentation

### Markdown Guidelines

All markdown files must follow these rules (enforced by markdownlint):

- Maximum line length: 120 characters
- Use ATX-style headers (`# Header`)
- Lists must be surrounded by blank lines
- Use reference-style links: `[text][ref]` with `[ref]: url` at document end
- **Exception**: `README.md` uses absolute URLs (it's included in the NuGet package)
- **Exception**: Agent markdown files in `.github/agents/*.md` use inline links `[text](url)` for visibility in agent
  context

### Spell Checking

All files are spell-checked using cspell. Add project-specific terms to `.cspell.json`:

```json
{
  "words": [
    "myterm"
  ]
}
```

## Quality Checks

Before submitting a pull request, ensure all quality checks pass:

### 1. Build and Test

```bash
dotnet build --configuration Release
dotnet test --configuration Release
```

All tests must pass with zero warnings.

### 2. Linting

Run all linters to ensure code quality and consistency:

```bash
# Markdown linting
npx markdownlint-cli2

# Spell checking
npx cspell "**/*.{md,cs}" --no-progress

# YAML linting
yamllint .

# Code formatting
dotnet format --verify-no-changes
```

All linters must pass with no errors or warnings.

### 3. Code Coverage

Maintain or improve code coverage. Use the `--collect "XPlat Code Coverage"` option when running tests.

## Commit Messages

Write clear, concise commit messages:

- Use present tense ("Add feature" not "Added feature")
- Use imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit first line to 72 characters
- Reference issues and pull requests when applicable

Examples:

- `Add support for custom report headers`
- `Fix crash when server URL is invalid`
- `Update documentation for --branch option`
- `Refactor argument parsing for better testability`

## Pull Request Process

1. **Update Documentation**: Update relevant documentation for your changes
2. **Add Tests**: Include tests that cover your changes
3. **Run Quality Checks**: Ensure all linters, tests, and builds pass
4. **Submit PR**: Create a pull request with a clear description
5. **Code Review**: Address feedback from maintainers
6. **Merge**: Once approved, a maintainer will merge your PR

### Pull Request Template

When creating a pull request, include:

- **Description**: What changes does this PR introduce?
- **Motivation**: Why are these changes needed?
- **Related Issues**: Link to any related issues
- **Testing**: How have you tested these changes?
- **Checklist**:
  - [ ] Tests added/updated
  - [ ] Documentation updated
  - [ ] All tests pass
  - [ ] Code follows style guidelines
  - [ ] No new warnings introduced

## Requirements Management

SonarMark uses [DemaConsulting.ReqStream][reqstream] for requirements traceability:

- All requirements are defined in `requirements.yaml`
- Each requirement should be linked to test cases
- Run `dotnet reqstream` to generate requirements documentation
- Use the `--enforce` flag to ensure all requirements have test coverage

## Release Process

Releases are managed by project maintainers. The process includes:

1. Version bump in project files
2. Tag the release in Git
3. Build and test across all supported platforms
4. Publish NuGet package
5. Create GitHub release with artifacts and release notes

## Getting Help

- **Questions**: Use [GitHub Discussions][discussions]
- **Bugs**: Report via [GitHub Issues][issues]
- **Security**: See [SECURITY.md][security] for vulnerability reporting

## License

By contributing to SonarMark, you agree that your contributions will be licensed under the MIT License.

Thank you for contributing to SonarMark!

[code-of-conduct]: https://github.com/demaconsulting/SonarMark/blob/main/CODE_OF_CONDUCT.md
[dotnet-download]: https://dotnet.microsoft.com/download
[csharp-conventions]: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
[reqstream]: https://github.com/demaconsulting/ReqStream
[discussions]: https://github.com/demaconsulting/SonarMark/discussions
[issues]: https://github.com/demaconsulting/SonarMark/issues
[security]: https://github.com/demaconsulting/SonarMark/blob/main/SECURITY.md
