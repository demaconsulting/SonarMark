# Pull Request

## Description

<!-- Provide a clear and concise description of your changes -->

## Type of Change

<!-- Mark the relevant option with an 'x' -->

- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update
- [ ] Code quality improvement

## Related Issues

<!-- Link to related issues using #issue_number -->

Closes #

## Pre-Submission Checklist

Before submitting this pull request, ensure you have completed the following:

### Build and Test

- [ ] Code builds successfully: `dotnet build --configuration Release`
- [ ] All tests pass: `dotnet test --configuration Release`
- [ ] Code produces zero warnings

### Code Quality

- [ ] Code formatting is correct: `dotnet format --verify-no-changes`
- [ ] New code has appropriate XML documentation comments
- [ ] Static analyzer warnings have been addressed

### Quality Checks

Please run the following checks before submitting:

- [ ] **Spell checker passes**: `cspell "**/*.{md,cs}"`
- [ ] **Markdown linter passes**: `markdownlint "**/*.md"`
- [ ] **YAML linter passes**: `yamllint '**/*.{yml,yaml}'`

### Testing

- [ ] Added unit tests for new functionality
- [ ] Updated existing tests if behavior changed
- [ ] All tests follow the AAA (Arrange, Act, Assert) pattern
- [ ] Test coverage is maintained or improved

### Documentation

- [ ] Updated README.md (if applicable)
- [ ] Updated ARCHITECTURE.md (if applicable)
- [ ] Added code examples for new features (if applicable)
- [ ] Updated requirements.yaml (if applicable)

## Additional Notes

<!-- Add any additional context, screenshots, or information that reviewers should know -->
