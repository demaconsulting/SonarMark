package SharedDependencies {
    doc /* Shared Packages consumed by SonarMark. */

    part def SonarMarkSharedPackage {
        doc /* Shared Package: an earlier SonarMark release consumed as a CI dependency of this repository's own build pipeline for self-validation and quality-report generation. */

        comment designRef /* Design: docs/design/shared/sonar-mark.md */
        comment verificationRef /* Verification: docs/verification/shared/sonar-mark.md */
        comment reqRef /* Requirements: docs/reqstream/shared/sonar-mark.yaml */
    }
}
