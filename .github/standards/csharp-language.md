---
name: C# Language
description: Follow these standards when developing C# source code.
globs: ["**/*.cs"]
---

# C# Language Development Standard

## Required Standards

Read these standards first before applying this standard:

- **`coding-principles.md`** - Universal coding principles and quality gates

# File Patterns

- **Source Files**: `**/*.cs`

# Literate Coding Example

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

# Code Formatting

- **Format entire solution**: `dotnet format`
- **Format specific project**: `dotnet format MyProject.csproj`
- **Format specific file**: `dotnet format --include MyFile.cs`

# Quality Checks

- [ ] Zero compiler warnings (`TreatWarningsAsErrors=true`)
- [ ] XmlDoc documentation complete on all members (public, internal, protected, private)
