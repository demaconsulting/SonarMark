# build.ps1
#
# PURPOSE:
#   Unified cross-platform build script for SonarMark.
#   Restores dependencies, builds the solution in Release configuration, and runs all unit tests,
#   either unconditionally (-All, the full local build and test run) or only when the changed files affect the build (-Changed).
#
# USAGE:
#   pwsh ./build.ps1 -Help                     Print the usage text and exit 0
#   pwsh ./build.ps1 -All                      Full build and all tests
#   pwsh ./build.ps1 -Changed [-Base <ref>]    Build and test only when files changed since <ref>
#                                              (default origin/main, else main) plus uncommitted changes
#                                              affect the build; documentation-only changes build nothing
#   Add -DryRun to any scope to print the selection without building anything.
#   With no arguments the usage text is printed and the script exits 1 (a scope must be chosen).
#
#   The solution has one library and one test project, so the scope is all or nothing: a change to
#   either project, a file linked into one, or a build-wide file (solution, project, props/targets,
#   global.json, .editorconfig, nuget.config, tool manifest, this script, scripts/ProjectScripts.psm1)
#   runs the full build and tests; other changes the build does not read select nothing.
#
# EXTENSION POINTS:
#   Search for "[PROJECT-SPECIFIC]" comments to find the designated locations
#   for adding project-specific build or test operations.
#
# MODIFICATION POLICY:
#   Only modify this file to add project-specific operations at the designated
#   [PROJECT-SPECIFIC] extension points. The reusable scoping logic lives in
#   scripts/ProjectScripts.psm1, which is shared across repositories and must not be edited here.
#   With no arguments the script prints the usage text and exits 1; -All must still build and run
#   the standard test suite (the agents rely on that). Never remove or weaken a test step to make it pass.

param(
    # Full local build and all tests.
    [switch]$All,

    # Build and test only when the changed files affect the build.
    [switch]$Changed,

    # Git reference to compare against for -Changed (default: origin/main, else main).
    [string]$Base,

    # Print the selection and exit without building.
    [switch]$DryRun,

    # Print the usage text and exit 0.
    [switch]$Help
)

Import-Module (Join-Path $PSScriptRoot 'scripts/ProjectScripts.psm1') -Force

# [PROJECT-SPECIFIC] Repository files that tests read at run time (through the repository root, not through a
# project item), mapped to the test project that reads them. SonarMark's tests read none.
$RuntimeReads = [ordered]@{}

# ==============================================================================
# USAGE AND SCOPE VALIDATION
# ==============================================================================

function Show-Usage {
    Write-ScriptUsage -Script 'build.ps1' `
        -Purpose 'build the solution (Release) and run the unit tests.' `
        -Options ([ordered]@{
            '-All'        = 'Full local build and all tests; cannot be combined with another scope.'
            '-Changed'    = 'Build and test only if files changed since the base, plus uncommitted files, affect the build.'
            '-Base <ref>' = 'Base reference for -Changed (default origin/main, else main).'
            '-DryRun'     = 'Print what would be built and tested, then exit 0 without running anything.'
            '-Help'       = 'Print this text and exit 0.'
        }) `
        -Examples @('-Changed', '-Changed -DryRun', '-All')
}

if ($Help) { Show-Usage; exit 0 }

$scopeProblems = Test-ScopeArguments -Base $Base -Scopes ([ordered]@{
        All     = $All.IsPresent
        Changed = $Changed.IsPresent
    })
if ($scopeProblems.Count -gt 0) {
    $scopeProblems | ForEach-Object { Write-Host "Error: $_" }
    Write-Host ""
    Show-Usage
    exit 1
}

# ==============================================================================
# SCOPED BUILD (-Changed)
# One library and one test project: either a changed file affects the build (full build and
# tests) or nothing is built.
# ==============================================================================

if ($Changed) {
    $graph = Get-ProjectGraph
    $selected = [System.Collections.Generic.HashSet[string]]::new()
    $changedFiles = Get-ChangedFiles -Base $Base
    if ($null -eq $changedFiles) {
        Write-Host "Full build required: the changed files could not be determined."
    } else {
        Write-Host "Changed files: $($changedFiles.Count)"
        $fullReason = Add-ChangedProjects -Graph $graph -Files $changedFiles -Selected $selected -RuntimeReads $RuntimeReads
        if ($fullReason) {
            Write-Host "Full build required: $fullReason."
        } elseif ($selected.Count -eq 0) {
            Write-Host "Nothing to build: no changed file is read by the build."
            exit 0
        } else {
            Write-Host "Affected projects: $(@($selected) -join ', ')"
        }
    }
}

if ($DryRun) { Write-Host "Dry run: would run the full build and all tests."; exit 0 }

# ==============================================================================
# FULL BUILD (-All, or a scoped run that selected something)
# ==============================================================================

$buildError = $false

Write-Host "Restoring dependencies..."
dotnet restore
if ($LASTEXITCODE -ne 0) { $buildError = $true }

Write-Host "Building..."
dotnet build --no-restore --configuration Release
if ($LASTEXITCODE -ne 0) { $buildError = $true }

Write-Host "Running tests..."
dotnet test --no-build --configuration Release --report-trx --results-directory artifacts/tests
if ($LASTEXITCODE -ne 0) { $buildError = $true }

# [PROJECT-SPECIFIC] Add additional build steps here (e.g., packaging, publishing).

if (-not $buildError) {
    Write-Host "Build and tests completed successfully!"
}

exit ($buildError ? 1 : 0)
