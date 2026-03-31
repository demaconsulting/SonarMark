# SelfTest Subsystem

## Overview

The SelfTest subsystem provides a built-in self-validation mode that verifies the
tool's own functionality without requiring a live SonarQube/SonarCloud server.

## Units

| Unit | Source File | Purpose |
|:-----|:-----------|:--------|
| Validation | `SelfTest/Validation.cs` | Self-validation runner, writes TRX and JUnit result files |

## Interfaces

The `Validation.Run(context)` static method is the primary interface. It accepts
a `Context` and runs four internal tests using a mock HTTP client.

## Interactions

1. `Program.Run` calls `Validation.Run(context)` when `--validate` flag is set
2. `Validation` creates a mock `SonarQubeClient` to exercise the SonarIntegration subsystem
3. Results are written to TRX or JUnit XML files if `--results` is specified
