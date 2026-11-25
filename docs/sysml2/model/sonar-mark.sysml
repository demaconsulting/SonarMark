package SonarMark {
    private import SoftwareItemMetadata::*;
    private import OtsDependencies::*;
    private import SharedPackages::*;

    public import SonarMark_Program::*;
    public import SonarMark_CliSubsystem::*;
    public import SonarMark_SonarIntegrationSubsystem::*;
    public import SonarMark_ReportGenerationSubsystem::*;
    public import SonarMark_SelfTestSubsystem::*;

    doc
    /*
     * SonarMark is a .NET command-line tool that generates comprehensive markdown reports
     * from SonarQube/SonarCloud analysis results.
     */

    part def SonarMarkSystem {
        doc /* The SonarMark system as a whole. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.SonarMark.Tests/IntegrationTests.cs");
            designFiles = ("docs/design/sonar-mark.md");
            verificationFiles = ("docs/verification/sonar-mark.md");
            requirementFiles = ("docs/reqstream/sonar-mark/sonar-mark.yaml");
        }

        part program : Program;
        part cli : CliSubsystem;
        part sonarIntegration : SonarIntegrationSubsystem;
        part reportGeneration : ReportGenerationSubsystem;
        part selfTest : SelfTestSubsystem;

        part testResults : TestResults;
        part helpCards : HelpCards;

        part sonarMarkSharedPackage : SonarMarkSharedPackage;
    }
}
