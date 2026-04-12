---
name: Coding Principles
description: Follow these standards when developing any software code.
---

# Coding Principles Standards

This document defines universal coding principles and quality standards for software development within
Continuous Compliance environments.

# Core Principles

## Literate Coding

All code MUST follow literate programming principles:

- **Intent Comments**: Every function/method begins with a comment explaining WHY (not what)
- **Logical Separation**: Complex functions use comments to separate logical blocks
- **Public Documentation**: All public interfaces have comprehensive documentation
- **Clarity Over Cleverness**: Code should be immediately understandable by team members

## Universal Code Architecture Principles

### Design Patterns

- **Single Responsibility**: Functions with focused, testable purposes
- **Dependency Injection**: External dependencies injected for testing
- **Pure Functions**: Minimize side effects and hidden state
- **Clear Interfaces**: Well-defined API contracts
- **Separation of Concerns**: Business logic separate from infrastructure
- **Repository Structure Adherence**: Before creating any new files, analyze the repository structure to
  understand established directory conventions and file placement patterns. Place new files in locations
  consistent with existing patterns.

### Compliance-Ready Code Structure

- **Documentation Standards**: Language-appropriate documentation required on ALL members
- **Error Handling**: Comprehensive error cases with appropriate exception handling and logging
- **Configuration**: Externalize settings for different compliance environments
- **Resource Management**: Proper resource cleanup using language-appropriate patterns

# Quality Gates

## Code Quality Standards

- [ ] Zero compiler warnings (use language-specific warning-as-error flags)
- [ ] All code follows literate programming style
- [ ] Language-appropriate documentation complete on all members
- [ ] Passes static analysis (language-specific tools)

## Universal Anti-Patterns

- **Skip Literate Coding**: Don't skip literate programming comments - they are required for maintainability
- **Ignore Compiler Warnings**: Don't ignore compiler warnings - they exist for quality enforcement
- **Hidden Dependencies**: Don't create untestable code with hidden dependencies
- **Hidden Functionality**: Don't implement functionality without requirement traceability
- **Monolithic Functions**: Don't write monolithic functions with multiple responsibilities
- **Overcomplicated Solutions**: Don't make solutions more complex than necessary - favor simplicity and clarity
- **Premature Optimization**: Don't optimize for performance before establishing correctness
- **Copy-Paste Programming**: Don't duplicate logic - extract common functionality into reusable components
- **Magic Numbers**: Don't use unexplained constants - either name them or add clear comments

# Language-Specific Implementation

For each detected language:

- **Load Standards**: Read the appropriate `{language}-language.md` file from `.github/standards/`
- **Apply Tooling**: Use language-specific formatting, linting, and build tools
- **Follow Conventions**: Apply language-specific naming, patterns, and best practices
- **Generate Documentation**: Use language-appropriate documentation format (XmlDoc, Doxygen, JSDoc, etc.)
