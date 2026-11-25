### HelpCommand

#### Verification Approach

`HelpCommand` is verified by unit tests in `test/DemaConsulting.SonarMark.Tests/Cli/HelpCommandTests.cs`.
Each test creates a help command with `HelpCommand.Create`, which writes to in-memory writers instead of
the console. No mocking is applied: the tests use the real help cards embedded in the tool assembly and
the real HelpCards library, a documented dependency of the unit.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `HelpCommandTests` pass with zero failures.
- The help verb prints the topic index.
- The shipped catalog passes validation with warnings treated as errors.
- Every card prints in lines of at most 120 characters.

#### Test Scenarios

**HelpVerbPrintsTopicIndex**: `TryRun` is called with `["help"]`; it answers with exit code 0 and the
output lists the command of every root topic.
This scenario is tested by `HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero`.

**ShippedCatalogPassesValidation**: The catalog configuration from `CreateOptions` is validated with
warnings treated as errors; no defect is reported, which proves that the root cards are exactly
`RootCardIds`, that every card carries one purpose tag, and that every command example parses as a
command line of the tool.
This scenario is tested by `HelpCommand_CreateOptions_ShippedCatalog_PassesValidation`.

**EveryCardFitsMaximumLineWidth**: Every card of the shipped catalog is printed with its links and
tokens expanded and a fixed, long prerelease version in place of the build's version; each exits with
code 0 and has no line longer than 120 characters.
This scenario is tested by `HelpCommand_TryRun_EveryCard_FitsMaximumLineWidth`.
