---
name: Software Items
description: Follow these standards when categorizing software components.
---

# Software Items Definition Standards

This document defines standards for categorizing software items within
Continuous Compliance environments because proper categorization determines
requirements management approach, testing strategy, and review scope.

# Software Item Categories

Categorize all software into five primary groups:

- **Software Package**: Distributable unit delivered to end users or dependent
  systems, containing one software system with all its components. All software
  systems are delivered as a software package. When consumed by another system,
  our software package is treated as an OTS Software Item by that system.
- **Software System**: Complete deliverable product including all components
  and external interfaces, contained within a software package
- **Software Subsystem**: Major architectural component with well-defined
  interfaces and responsibilities
- **Software Unit**: Individual class, function, or tightly coupled set of
  functions that can be tested in isolation
- **OTS Software Item**: Third-party component (library, framework, tool, or
  published software package) providing functionality not developed in-house

**Naming**: When names collide in hierarchy, add descriptive suffix to higher-level entity:

- Package: Package (e.g. TestResults → TestResultsPackage)
- System: Application/Library/System (e.g. TestResults → TestResultsLibrary)
- Subsystem: Subsystem (e.g. Linter → LinterSubsystem)

# Naming Conventions in File Path Patterns

Two placeholder styles appear in path patterns across these standards:

- **Kebab-case** (`{system-name}`, `{unit-name}`): always kebab-case -
  used in documentation and requirements paths
- **Cased** (`{SystemName}`, `{UnitName}`): follow your language's convention -
  `PascalCase` for C#/Java, `snake_case` for C++/Python -
  used in source and test file paths

# Categorization Guidelines

Choose the appropriate category based on scope and testability:

## Software Package

- Represents one distributable artifact
  (e.g., NuGet package, npm package, Docker image, installer)
- Contains exactly one software system with its subsystems and units
- Tested through package-level acceptance and integration tests

## Software System

- Represents the entire product boundary
- Tested through system integration and end-to-end tests

## Software Subsystem

- Major architectural boundary (authentication, data layer, UI, communications)
- Contains software units and optionally child subsystems
- Subsystems may nest when a component has distinct internal boundaries
- Typically maps to project folders or namespaces
- Tested through subsystem integration tests

## Software Unit

- Smallest independently testable component
- Typically a single class or cohesive set of functions
- Methods within a class are NOT separate units
- Tested through unit tests with mocked dependencies

## OTS Software Item

- External dependency not developed in-house - typically a third-party published
  software package (NuGet, npm, etc.), hosted service, or tool
- Our own published software package becomes an OTS item to any system that
  consumes it
- Tested through integration tests proving required functionality works
- Examples: System.Text.Json, Entity Framework, third-party APIs

# Software Item Artifact Model

Each software item has four artifact types that together form a complete review
unit - because reviewing any one artifact in isolation cannot determine whether
the item is correct, well-designed, and proven to work:

- **Requirements** - WHAT the item must do (drives all other artifacts; applies to all item types)
- **Design** - HOW the item satisfies its requirements (in-house items only: system, subsystem, unit)
- **Source code** - The implementation of the design (in-house units only)
- **Tests** - PROOF the item does WHAT it is required to do (applies to all item types)
