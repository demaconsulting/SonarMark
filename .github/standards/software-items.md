---
name: Software Items
description: Follow these standards when categorizing software components.
---

# Software Items Definition Standards

This document defines DEMA Consulting standards for categorizing software
items within Continuous Compliance environments because proper categorization
determines requirements management approach, testing strategy, and review
scope.

# Software Item Categories

Categorize all software into four primary groups:

- **Software System**: Complete deliverable product including all components
  and external interfaces
- **Software Subsystem**: Major architectural component with well-defined
  interfaces and responsibilities
- **Software Unit**: Individual class, function, or tightly coupled set of
  functions that can be tested in isolation
- **OTS Software Item**: Third-party component (library, framework, tool)
  providing functionality not developed in-house

**Naming**: When names collide in hierarchy, add descriptive suffix to higher-level entity:

- System: Application/Library/System (e.g. TestResults → TestResultsLibrary)
- Subsystem: Subsystem (e.g. Linter → LinterSubsystem)

# Categorization Guidelines

Choose the appropriate category based on scope and testability:

## Software System

- Represents the entire product boundary
- Tested through system integration and end-to-end tests

## Software Subsystem

- Major architectural boundary (authentication, data layer, UI, communications)
- Contains multiple software units working together
- Typically maps to project folders or namespaces
- Tested through subsystem integration tests

## Software Unit

- Smallest independently testable component
- Typically a single class or cohesive set of functions
- Methods within a class are NOT separate units
- Tested through unit tests with mocked dependencies

## OTS Software Item

- External dependency not developed in-house
- Tested through integration tests proving required functionality works
- Examples: System.Text.Json, Entity Framework, third-party APIs
