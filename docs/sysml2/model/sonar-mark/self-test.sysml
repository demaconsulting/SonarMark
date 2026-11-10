package SonarMark {
    private import SoftwareItemMetadata::*;

    part def SelfTestSubsystem {
        doc /* Self-validation test runner exercised at startup for OTS/BuildMark self-check evidence. */

        metadata info : SoftwareInfo {
            description = "Self-validation test runner exercised at startup for OTS/BuildMark self-check evidence.";
            testFiles = ("test/DemaConsulting.SonarMark.Tests/SelfTest/SelfTestTests.cs");
            designFiles = ("docs/design/sonar-mark/self-test.md");
            verificationFiles = ("docs/verification/sonar-mark/self-test.md");
            requirementFiles = ("docs/reqstream/sonar-mark/self-test/self-test.yaml");
        }

        part validation : Validation;
    }
}
