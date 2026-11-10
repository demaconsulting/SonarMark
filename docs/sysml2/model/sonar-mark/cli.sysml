package SonarMark {
    private import SoftwareItemMetadata::*;

    part def CliSubsystem {
        doc /* Command-line argument parsing and program-output routing. */

        metadata info : SoftwareInfo {
            description = "Command-line argument parsing and program-output routing.";
            testFiles = ("test/DemaConsulting.SonarMark.Tests/Cli/CliTests.cs");
            designFiles = ("docs/design/sonar-mark/cli.md");
            verificationFiles = ("docs/verification/sonar-mark/cli.md");
            requirementFiles = ("docs/reqstream/sonar-mark/cli/cli.yaml");
        }

        part context : Context;
    }
}
