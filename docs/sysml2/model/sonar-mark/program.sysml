package SonarMark {
    private import SoftwareItemMetadata::*;

    part def Program {
        doc /* Entry point and execution orchestrator. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.SonarMark/Program.cs");
            testFiles = ("test/DemaConsulting.SonarMark.Tests/ProgramTests.cs");
            designFiles = ("docs/design/sonar-mark/program.md");
            verificationFiles = ("docs/verification/sonar-mark/program.md");
            requirementFiles = ("docs/reqstream/sonar-mark/program.yaml");
        }
    }
}
