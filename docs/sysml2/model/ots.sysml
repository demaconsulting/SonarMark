package OtsDependencies {
    private import SoftwareItemMetadata::*;

    doc /* Off-the-shelf (OTS) dependencies used by SonarMark. */

    part def TestResults {
        doc /* OTS: DemaConsulting.TestResults. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/test-results.md");
            verificationFiles = ("docs/verification/ots/test-results.md");
            requirementFiles = ("docs/reqstream/ots/test-results.yaml");
        }
    }

    part def HelpCards {
        doc /* OTS: DemaConsulting.HelpCards. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/help-cards.md");
            verificationFiles = ("docs/verification/ots/help-cards.md");
            requirementFiles = ("docs/reqstream/ots/help-cards.yaml");
        }
    }
}
