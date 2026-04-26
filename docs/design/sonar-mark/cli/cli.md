# CLI Subsystem

## Overview

The CLI subsystem handles command-line argument parsing and program output routing.
It is the entry point for the SonarMark tool, parsing arguments and dispatching
to the appropriate functionality.

## Units

| Unit | Source File | Purpose |
| :--- | :---------- | :------ |
| Context | `Cli/Context.cs` | Argument parsing, output, log-file, enforce, results-file |

## Interfaces

The CLI subsystem exposes the `Context` class which carries all parsed arguments
and program state. `Program` creates a `Context` instance and passes it to all
other subsystems.

## Interactions

1. `Program.Main` creates a `Context` via `Context.Create(args)`
2. `Program.Run` dispatches based on context flags (Version, Help, Validate, or analysis)
3. All subsystems receive the `Context` for output and configuration
