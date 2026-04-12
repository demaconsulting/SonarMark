---
name: Testing Principles
description: Follow these standards when developing any software tests.
---

# Testing Principles Standards

This document defines universal testing principles and quality standards for test development within
Continuous Compliance environments.

# Test Dependency Boundaries (MANDATORY)

Respect software item hierarchy boundaries to ensure review-sets can validate proper architectural scope.

- **System Tests**: May use functionality from any subsystem or unit within the system
- **Subsystem Tests**: May only use units within the subsystem + documented dependencies in design docs
- **Unit Tests**: May only test the unit + documented dependencies in design docs

Undocumented cross-hierarchy dependencies indicate either missing design documentation or architectural violations.

# AAA Pattern (MANDATORY)

All tests MUST follow Arrange-Act-Assert pattern with descriptive comments because regulatory reviews
require clear test logic that can be independently verified against requirements.

# Language-Specific Implementation

Load the appropriate `{language}-testing.md` file for framework-specific implementation details,
file organization patterns, and tooling requirements.

# Quality Gates

- [ ] Tests respect software item hierarchy boundaries (System/Subsystem/Unit scope)
- [ ] Cross-hierarchy test dependencies documented in design documentation
- [ ] All tests follow AAA pattern with descriptive comments
- [ ] Test names follow hierarchical naming conventions for requirement linkage
- [ ] Tests linkable to requirements through ReqStream
- [ ] Platform-specific tests use appropriate source filters
- [ ] Both success and error scenarios covered
- [ ] External dependencies properly mocked for isolation
