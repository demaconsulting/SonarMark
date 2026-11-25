## DemaConsulting.HelpCards

### Verification Approach

`DemaConsulting.HelpCards` is a runtime library with no self-validation mode, so it is verified
through tests in the SonarMark test suite that use the real package and the help cards embedded
in the tool assembly. The unit tests drive `HelpCommand` with in-memory writers, and the
integration tests run the built tool. No mocking is applied.

The tests verify that:

- the help verb is answered from the embedded cards, in-process and by the built tool;
- the validator accepts the shipped cards with warnings treated as errors;
- the example runner runs every command example against the built tool and finds the result that
  its card states. The runner does not run `snippet` examples, which SonarMark uses for the
  commands that need a live SonarQube or SonarCloud server.

### Test Environment

Tests require:

- No network access; no help example contacts a server.
- A writable temporary directory, in which the example runner creates its scratch folders.
- The `DemaConsulting.HelpCards` NuGet package installed as a package reference in the
  production project (`src/DemaConsulting.SonarMark/DemaConsulting.SonarMark.csproj`).

### Acceptance Criteria

The OTS integration is accepted when all linked tests pass with zero failures.

### Test Scenarios

**HelpCommand**: The help verb is answered from the embedded cards, in-process and by the built
tool. Tested by `HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero` and
`IntegrationTest_HelpTopics_HelpVerb_ListsTopics`.

**TestHelpers**: The validator accepts the shipped cards, and the example runner runs every command
example against the built tool. Tested by `HelpCommand_CreateOptions_ShippedCatalog_PassesValidation`
and `IntegrationTest_HelpExamples_AllCommandExamples_GiveStatedResults`.
