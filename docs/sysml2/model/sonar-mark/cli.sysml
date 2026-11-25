package SonarMark_CliSubsystem {
    private import SoftwareItemMetadata::*;
    public import SonarMark_Context::*;
    public import SonarMark_HelpCommand::*;

    part def CliSubsystem {
        doc /* Command-line argument parsing, program-output routing, and the built-in help command. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.SonarMark.Tests/Cli/CliTests.cs");
            designFiles = ("docs/design/sonar-mark/cli.md");
            verificationFiles = ("docs/verification/sonar-mark/cli.md");
            requirementFiles = ("docs/reqstream/sonar-mark/cli/cli.yaml");
        }

        part context : Context;
        part helpCommand : HelpCommand;
    }
}
