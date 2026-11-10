package SonarMark {
    private import SoftwareItemMetadata::*;

    part def Validation {
        doc /* Self-validation runner that writes TRX and JUnit result files. */

        metadata info : SoftwareInfo {
            description = "Self-validation runner that writes TRX and JUnit result files.";
            sourceFiles = ("src/DemaConsulting.SonarMark/SelfTest/Validation.cs");
            testFiles = ("test/DemaConsulting.SonarMark.Tests/SelfTest/ValidationTests.cs");
            designFiles = ("docs/design/sonar-mark/self-test/validation.md");
            verificationFiles = ("docs/verification/sonar-mark/self-test/validation.md");
            requirementFiles = ("docs/reqstream/sonar-mark/self-test/validation.yaml");
        }
    }
}
