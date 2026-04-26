# SelfTest Subsystem

## Overview

The SelfTest subsystem provides a built-in self-validation mode that verifies the
tool's own functionality without requiring a live SonarQube/SonarCloud server.

## Units

| Unit | Source File | Purpose |
| :--- | :---------- | :------ |
| Validation | `SelfTest/Validation.cs` | Self-validation runner, writes TRX and JUnit result files |

## Interfaces

The `Validation.Run(context)` static method is the primary interface. It accepts
a `Context` and runs four internal tests using a mock HTTP client.

## Interactions

1. `Program.Run` calls `Validation.Run(context)` when `--validate` flag is set
2. `Validation` creates a mock `SonarQubeClient` to exercise the SonarIntegration subsystem
3. Results are written to TRX or JUnit XML files if `--results` is specified

## Error Handling

Validation failures are surfaced through the `Context` output mechanism:

- Individual test failures are reported via `context.WriteError`, which sets the internal
  `_hasErrors` flag and causes `context.ExitCode` to return `1`.
- If result-file writing fails (e.g., the output path is not writable), an
  `InvalidOperationException` is thrown and propagates to `Program.Main`, which
  catches it, prints the message, and returns exit code `1`.
- The subsystem does not swallow exceptions from the mocked `SonarQubeClient`;
  any unexpected exception propagates to `Main` and is re-thrown after printing
  so that CI runners receive a non-zero exit code.

## Satisfies Requirements

- `SonarMark-Subsystem-SelfTest` — `Validation.Run` exercises the full analysis
  pipeline using a mock HTTP client, writes test results, and reports pass/fail
  status through the context exit code
