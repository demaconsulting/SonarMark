package SharedDependencies {
    private import SoftwareItemMetadata::*;

    doc /* Shared Packages consumed by SonarMark. */

    part def SonarMarkSharedPackage {
        doc /* Shared Package: an earlier SonarMark release consumed as a CI dependency of this repository's own build pipeline for self-validation and quality-report generation. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/shared/sonar-mark.md");
            verificationFiles = ("docs/verification/shared/sonar-mark.md");
            requirementFiles = ("docs/reqstream/shared/sonar-mark.yaml");
        }
    }
}
