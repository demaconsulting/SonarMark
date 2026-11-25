package SonarMark_HelpCommand {
    private import SoftwareItemMetadata::*;
    private import OtsDependencies::*;

    part def HelpCommand {
        doc /* Built-in help command: answers the help verb from the help cards embedded in the assembly. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.SonarMark/Cli/HelpCommand.cs");
            testFiles = ("test/DemaConsulting.SonarMark.Tests/Cli/HelpCommandTests.cs");
            designFiles = ("docs/design/sonar-mark/cli/help-command.md");
            verificationFiles = ("docs/verification/sonar-mark/cli/help-command.md");
            requirementFiles = ("docs/reqstream/sonar-mark/cli/help-command.yaml");
        }
    }

    // Serves the help command from the cards embedded in the assembly
    dependency HelpCommand to HelpCards;
}
