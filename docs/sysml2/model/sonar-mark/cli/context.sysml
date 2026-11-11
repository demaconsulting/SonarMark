package SonarMark {
    private import SoftwareItemMetadata::*;

    part def Context {
        doc /* Command-line argument parser and I/O owner for SonarMark. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.SonarMark/Cli/Context.cs");
            testFiles = ("test/DemaConsulting.SonarMark.Tests/Cli/ContextTests.cs");
            designFiles = ("docs/design/sonar-mark/cli/context.md");
            verificationFiles = ("docs/verification/sonar-mark/cli/context.md");
            requirementFiles = ("docs/reqstream/sonar-mark/cli/context.yaml");
        }
    }
}
