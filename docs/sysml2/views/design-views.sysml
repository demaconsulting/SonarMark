// Views rendered into docs/design/generated/ for the Design document.
// Imports the SonarMark system facade (which re-exports every subsystem and unit package)
// so each 'expose' statement can reference its target part def directly. Per SysML2Tools
// syntax, 'expose' is only valid inside a named 'view' usage (not a 'view def' definition),
// and it - not 'render' - is what scopes the rendered diagram content.
package SonarMarkDesignViews {
    private import StandardViewDefinitions::*;
    private import SonarMark::*;

    view SoftwareStructureView : InterconnectionView {
        expose SonarMark::SonarMarkSystem;
    }

    view SonarMarkView {
        expose SonarMarkSystem;
    }

    view CliView {
        expose CliSubsystem;
    }

    view SonarIntegrationView {
        expose SonarIntegrationSubsystem;
    }

    view ReportGenerationView {
        expose ReportGenerationSubsystem;
    }

    view SelfTestView {
        expose SelfTestSubsystem;
    }
}
