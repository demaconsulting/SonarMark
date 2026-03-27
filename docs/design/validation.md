# Validation

## Overview

`Validation` provides the self-validation capability of SonarMark. When the tool
is invoked with `--validate`, `Validation.Run` executes a suite of internal tests
using a mocked HTTP client, verifying that the tool's core workflows function
correctly without starting a real HTTP server or requiring a live SonarQube/SonarCloud instance.

## Design Decisions

### Mock HTTP Client

Validation uses a mock `HttpClient` that intercepts all HTTP requests and returns
pre-defined JSON responses. This allows the full SonarQubeClient code path to be
exercised without any external network dependency.

### TestResults Integration

Results are collected into a `DemaConsulting.TestResults.TestResults` instance and
optionally written to a TRX or JUnit XML file via `DemaConsulting.TestResults.IO`.
This makes validation output compatible with standard CI test result consumers
and feeds into the requirements traceability pipeline.

### Static Class

`Validation` is a static class because it has no instance state; all required
context is passed explicitly as parameters. This avoids unnecessary object
allocation and makes the dependency on `Context` explicit.

### Per-Test Helper Methods

Each validation scenario (quality gate retrieval, issues retrieval, hot-spots
retrieval, markdown report generation) is isolated in its own helper method.
This makes failures easy to diagnose and allows individual scenarios to be
added or removed independently.

## Satisfies Requirements

- `SonarMark-Validation-Run` — implements the self-validation mode invoked by `--validate`
- `SonarMark-Validation-Results` — writes results to the file specified by `--results`
- `SonarMark-Validation-TrxFormat` — writes TRX format when the results file has a `.trx` extension
- `SonarMark-Validation-JUnitFormat` — writes JUnit XML when the results file has a `.xml` extension
