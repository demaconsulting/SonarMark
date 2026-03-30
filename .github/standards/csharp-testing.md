# C# Testing Standards (MSTest)

This document defines DEMA Consulting standards for C# test development using
MSTest within Continuous Compliance environments.

# AAA Pattern Implementation (MANDATORY)

Structure all tests using Arrange-Act-Assert pattern because regulatory reviews
require clear test logic that can be independently verified against
requirements.

```csharp
[TestMethod]
public void ServiceName_MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - (description)
    // TODO: Set up test data, mocks, and system under test.

    // Act - (description)
    // TODO: Execute the action being tested

    // Assert - (description)
    // TODO: Verify expected outcomes and interactions
}
```

# Test Naming Standards

Use descriptive test names because test names appear in requirements traceability matrices and compliance reports.

- **Pattern**: `ClassName_MethodUnderTest_Scenario_ExpectedBehavior`
- **Descriptive Scenarios**: Clearly describe the input condition being tested
- **Expected Behavior**: State the expected outcome or exception

## Examples

- `UserValidator_ValidateEmail_ValidFormat_ReturnsTrue`
- `UserValidator_ValidateEmail_InvalidFormat_ThrowsArgumentException`
- `PaymentProcessor_ProcessPayment_InsufficientFunds_ReturnsFailureResult`

# Requirements Coverage

Link tests to requirements because every requirement must have passing test evidence for compliance validation.

- **ReqStream Integration**: Tests must be linkable in requirements YAML files
- **Platform Filters**: Use source filters for platform-specific requirements (`windows@TestName`)
- **TRX Format**: Generate test results in TRX format for ReqStream compatibility
- **Coverage Completeness**: Test both success paths and error conditions

# Mock Dependencies

Mock external dependencies using NSubstitute (preferred) because tests must run in isolation to generate
reliable evidence.

- **Isolate System Under Test**: Mock all external dependencies (databases, web services, file systems)
- **Verify Interactions**: Assert that expected method calls occurred with correct parameters
- **Predictable Behavior**: Set up mocks to return known values for consistent test results

# MSTest V4 Antipatterns

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
- [ ] Test names follow `ClassName_MethodUnderTest_Scenario_ExpectedBehavior`
- [ ] Each test verifies single, specific behavior (no shared state)
- [ ] Both success and failure scenarios covered including edge cases
- [ ] External dependencies mocked with NSubstitute or equivalent
- [ ] Tests linked to requirements with source filters where needed
- [ ] Test results generate TRX format for ReqStream compatibility
- [ ] MSTest V4 antipatterns avoided (proper assertions, public visibility, etc.)
