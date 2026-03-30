# C# Language Coding Standards

This document defines DEMA Consulting standards for C# software development
within Continuous Compliance environments.

## Literate Programming Style (MANDATORY)

Write all C# code in literate style because regulatory environments require
code that can be independently verified against requirements by reviewers.

- **Intent Comments**: Start every code paragraph with a comment explaining
  intent (not mechanics). Enables verification that code matches requirements.
- **Logical Separation**: Use blank lines to separate logical code paragraphs.
  Makes algorithm structure visible to reviewers.
- **Purpose Over Process**: Comments describe why, code shows how. Separates
  business logic from implementation details.
- **Standalone Clarity**: Reading comments alone should explain the algorithm
  approach. Supports independent code review.

### Example

```csharp
// Validate input parameters to prevent downstream errors
if (string.IsNullOrEmpty(input))
{
    throw new ArgumentException("Input cannot be null or empty", nameof(input));
}

// Transform input data using the configured processing pipeline
var processedData = ProcessingPipeline.Transform(input);

// Apply business rules and validation logic
var validatedResults = BusinessRuleEngine.ValidateAndProcess(processedData);

// Return formatted results matching the expected output contract
return OutputFormatter.Format(validatedResults);
```

## XML Documentation (MANDATORY)

Document ALL members (public, internal, private) with XML comments because
compliance documentation is auto-generated from source code comments and review
agents need to validate implementation against documented intent.

## Dependency Management

Structure code for testability because all functionality must be validated
through automated tests linked to requirements.

### Rules

- **Inject Dependencies**: Use constructor injection for all external dependencies.
  Enables mocking for unit tests.
- **Avoid Static Dependencies**: Use dependency injection instead of static
  calls. Makes code testable in isolation.
- **Single Responsibility**: Each class should have one reason to change.
  Simplifies testing and requirements traceability.
- **Pure Functions**: Minimize side effects and hidden state. Makes behavior
  predictable and testable.

## Error Handling

Implement comprehensive error handling because failures must be logged for
audit trails and compliance reporting.

- **Validate Inputs**: Check all parameters and throw appropriate exceptions
  with clear messages
- **Use Typed Exceptions**: Throw specific exception types
  (`ArgumentException`, `InvalidOperationException`) for different error
  conditions
- **Include Context**: Exception messages should include enough information
  for troubleshooting
- **Log Appropriately**: Use structured logging for audit trails in regulated
  environments

## Quality Checks

Before submitting C# code, verify:

- [ ] Code follows Literate Programming Style rules (intent comments, logical separation)
- [ ] XML documentation on ALL members with required tags
- [ ] Dependencies injected via constructor (no static dependencies)
- [ ] Single responsibility principle followed (one reason to change)
- [ ] Input validation with typed exceptions and clear messages
- [ ] Zero compiler warnings with `TreatWarningsAsErrors=true`
- [ ] Compatible with ReqStream requirements traceability
