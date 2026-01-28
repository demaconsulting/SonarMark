# Introduction

This document contains the requirements trace matrix for the SonarMark project.

## Purpose

The trace matrix links requirements to their corresponding test cases, ensuring complete
test coverage and traceability from requirements to implementation.

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
