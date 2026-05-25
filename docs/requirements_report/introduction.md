# Introduction

This document provides the requirements Trace Matrix for SonarMark,
mapping each requirement to its corresponding test evidence.

## Purpose

To demonstrate that every requirement is covered by at least one passing test,
providing compliance evidence for SonarMark.

## Scope

This document covers all requirements in `docs/reqstream/` and their test evidence,
including:

- Linkage between requirements and test cases
- Test coverage status for each requirement
- Both unit/integration tests and self-validation tests
- Platform-specific test execution results across Windows, Linux, and macOS

## References

N/A

## Test Sources

Requirements traceability in SonarMark uses two types of tests:

- **Unit and Integration Tests**: Standard MSTest tests that verify code functionality
- **Self-Validation Tests**: Built-in validation tests run via `sonarmark --validate --results`

To generate complete traceability reports, both test result files must be included:

```bash
# Run unit and integration tests
dotnet test --configuration Release --results-directory test-results --logger "trx"

# Run validation tests
sonarmark --validate --results validation.trx

# Verify requirements traceability
dotnet reqstream --requirements requirements.yaml \
  --tests "test-results/**/*.trx" \
  --tests validation.trx \
  --enforce
```

## Interpretation

The trace matrix shows:

- **Requirement ID**: Unique identifier for each requirement
- **Requirement Title**: Brief description of the requirement
- **Test Coverage**: List of test cases that verify the requirement
- **Status**: Indication of whether all mapped tests pass

## Coverage Requirements

All requirements must have:

- At least one test case mapped to verify the requirement
- All mapped tests must pass for the requirement to be satisfied
- Tests must execute on supported platforms and .NET runtimes
