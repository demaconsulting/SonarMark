---
name: C# Testing
description: Follow these standards when developing C# tests.
globs: ["**/test/**/*.cs", "**/tests/**/*.cs", "**/*Tests.cs", "**/*Test.cs"]
---

# C# Testing Standards (MSTest)

This document defines standards for C# test development using
MSTest within Continuous Compliance environments.

## Required Standards

Read these standards first before applying this standard:

- **`testing-principles.md`** - Universal testing principles and dependency boundaries
- **`csharp-language.md`** - C# language development standards

# C# AAA Pattern Implementation

```csharp
[TestMethod]
public void ServiceName_MethodName_Scenario_ExpectedBehavior()
{
    // Arrange: description of setup (omit if nothing to set up)

    // Act: description of action (can combine with Assert when action occurs within assertion)

    // Assert: description of verification
}
```

# Test Naming Standards

Use descriptive test names because test names appear in requirements traceability matrices and compliance reports.

- **System tests**: `{SystemName}_{Functionality}_{Scenario}_{ExpectedBehavior}`
- **Subsystem tests**: `{SubsystemName}_{Functionality}_{Scenario}_{ExpectedBehavior}`
- **Unit tests**: `{ClassName}_{MethodUnderTest}_{Scenario}_{ExpectedBehavior}`
- **Descriptive Scenarios**: Clearly describe the input condition being tested
- **Expected Behavior**: State the expected outcome or exception

## Examples

- `UserValidator_ValidateEmail_ValidFormat_ReturnsTrue`
- `UserValidator_ValidateEmail_InvalidFormat_ThrowsArgumentException`
- `PaymentProcessor_ProcessPayment_InsufficientFunds_ReturnsFailureResult`

# Mock Dependencies

Mock external dependencies using NSubstitute (preferred) because tests must run in isolation to generate
reliable evidence.

- **Isolate System Under Test**: Mock all external dependencies (databases, web services, file systems)
- **Verify Interactions**: Assert that expected method calls occurred with correct parameters
- **Predictable Behavior**: Set up mocks to return known values for consistent test results

# MSTest V4 Anti-patterns

Avoid these common MSTest V4 patterns because they produce poor error messages or cause tests to be silently ignored.

# Avoid Assertions in Catch Blocks (MSTEST0058)

Instead of wrapping code in try/catch and asserting in the catch block, use `Assert.ThrowsExactly<T>()`:

```csharp
var ex = Assert.ThrowsExactly<ArgumentNullException>(() => SomeWork());
Assert.Contains("Some message", ex.Message);
```

# Avoid Assert.IsTrue/IsFalse for Equality Checks

Use `Assert.AreEqual`/`Assert.AreNotEqual` instead, as they provide better failure messages:

```csharp
// ❌ Bad: Assert.IsTrue(result == expected);
// ✅ Good: Assert.AreEqual(expected, result);
```

# Avoid Non-Public Test Classes and Methods

Test classes and `[TestMethod]` methods must be `public` or they will be silently ignored:

```csharp
// ❌ Bad: internal class MyTests
// ✅ Good: public class MyTests
```

# Avoid Assert.IsTrue for Collection Count

Use `Assert.HasCount` for count assertions:

```csharp
// ❌ Bad: Assert.IsTrue(collection.Count == 3);
// ✅ Good: Assert.HasCount(3, collection);
```

# Avoid Assert.IsTrue for String Prefix Checks

Use `Assert.StartsWith` instead, as it produces clearer failure messages:

```csharp
// ❌ Bad: Assert.IsTrue(value.StartsWith("prefix"));
// ✅ Good: Assert.StartsWith("prefix", value);
```

# Quality Checks

Before submitting C# tests, verify:

- [ ] All tests follow AAA pattern with clear section comments
- [ ] Test names follow hierarchical patterns defined in Test Naming Standards section
- [ ] Each test verifies single, specific behavior (no shared state)
- [ ] Both success and failure scenarios covered including edge cases
- [ ] External dependencies mocked with NSubstitute or equivalent
- [ ] Tests linked to requirements with source filters where needed
- [ ] Test results generate TRX format for ReqStream compatibility
- [ ] MSTest V4 anti-patterns avoided (proper assertions, public visibility, etc.)
