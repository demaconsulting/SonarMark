# Template Consistency Analysis for SonarMark

**Date:** 2025-01-13  
**Template Repository:** [TemplateDotNetTool](https://github.com/demaconsulting/TemplateDotNetTool)  
**Reference Implementation:** [BuildMark](https://github.com/demaconsulting/BuildMark)

## Executive Summary

This analysis compares SonarMark against the TemplateDotNetTool template to identify required updates, with special focus on the new **tool version generation** feature using VersionMark.

### Key Findings

1. ✅ **SonarMark already installs itself globally** during integration-test and build-docs jobs
2. ❌ **Missing VersionMark integration** - The template now includes comprehensive tool version tracking
3. ❌ **Missing `.versionmark.yaml` configuration file**
4. ⚠️ **Tool list needs update** - Missing `demaconsulting.versionmark` in `.config/dotnet-tools.json`
5. ⚠️ **SonarMark entry in `.versionmark.yaml` needs special handling** for global installation

---

## 1. Tool Version Generation with VersionMark

### What is VersionMark?

VersionMark is a new tool from DEMA Consulting that:
- **Captures** tool versions across multiple build jobs
- **Aggregates** version information from distributed builds
- **Reports** a comprehensive version manifest for build transparency
- Supports tools installed **locally** (via `dotnet tool restore`) and **globally** (via `dotnet tool install --global`)

### How It Works in the Template

The workflow follows this pattern:

#### Step 1: Capture Phase (Each Job)

Each workflow job captures its tool versions:

```yaml
- name: Capture tool versions
  shell: bash
  run: |
    echo "Capturing tool versions..."
    dotnet versionmark --capture --job-id "quality" -- dotnet git versionmark
    echo "✓ Tool versions captured"

- name: Upload version capture
  uses: actions/upload-artifact@v6
  with:
    name: version-capture-quality
    path: versionmark-quality.json
```

Key points:
- Each job gets a unique `--job-id` (e.g., "quality", "build-win", "int-ubuntu-8")
- Job ID is embedded in the output filename: `versionmark-{job-id}.json`
- Tools are listed after `--` separator
- Artifacts are uploaded for later aggregation

#### Step 2: Publish Phase (build-docs Job)

The `build-docs` job aggregates all captures and generates a report:

```yaml
- name: Download all version captures
  uses: actions/download-artifact@v7
  with:
    path: version-captures
    pattern: 'version-capture-*'

- name: Capture tool versions for build-docs
  shell: bash
  run: |
    echo "Capturing tool versions..."
    dotnet versionmark --capture --job-id "build-docs" -- \
      dotnet git node npm pandoc weasyprint sarifmark sonarmark reqstream buildmark versionmark
    echo "✓ Tool versions captured"

- name: Publish Tool Versions
  shell: bash
  run: |
    echo "Publishing tool versions..."
    dotnet versionmark --publish --report docs/buildnotes/versions.md --report-depth 1 \
      -- "versionmark-*.json" "version-captures/**/versionmark-*.json"
    echo "✓ Tool versions published"
```

Key points:
- Downloads all version capture artifacts from previous jobs
- Captures its own tool versions
- Uses `--publish` mode to aggregate all captures
- Generates markdown report at `docs/buildnotes/versions.md`

### Configuration: `.versionmark.yaml`

VersionMark uses a YAML configuration to define how to extract versions:

```yaml
---
# VersionMark Configuration File
# This file defines which tools to capture and how to extract their version information.

tools:
  # .NET SDK
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+(?:\.\d+)?)'

  # Git
  git:
    command: git --version
    regex: '(?i)git version (?<version>\d+\.\d+\.\d+)'
  
  # ... (other tools)

  # SonarMark (DemaConsulting.SonarMark from dotnet tool list)
  sonarmark:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.sonarmark\s+(?<version>\d+\.\d+\.\d+)'
```

**Important for SonarMark:** When a tool installs itself globally (like BuildMark and SonarMark), the configuration must check the global tool list:

```yaml
  # SonarMark - checks global installation since it installs itself
  sonarmark:
    command: dotnet tool list --global
    regex: '(?i)demaconsulting\.sonarmark\s+(?<version>\d+\.\d+\.\d+)'
```

---

## 2. What SonarMark Already Has (Correctly)

### ✅ Global Installation Pattern

SonarMark **already** installs itself globally in two jobs:

1. **integration-test job** (lines 215-222):
```yaml
- name: Install tool from package
  shell: bash
  run: |
    echo "Installing package version ${{ inputs.version }} from: packages/"
    dotnet tool install --global \
      --add-source packages \
      --version ${{ inputs.version }} \
      DemaConsulting.SonarMark
```

2. **build-docs job** (lines 323-330):
```yaml
- name: Install SonarMark from package
  shell: bash
  run: |
    echo "Installing SonarMark version ${{ inputs.version }}"
    dotnet tool install --global \
      --add-source packages \
      --version ${{ inputs.version }} \
      DemaConsulting.SonarMark
```

This is **correct** and matches the BuildMark pattern! ✅

---

## 3. Changes Required for SonarMark

### 3.1 Add `.versionmark.yaml` Configuration File

**File:** `.versionmark.yaml` (root directory)

**Action:** Create new file with tool version extraction rules.

**Special Note:** SonarMark installs itself globally, so use `dotnet tool list --global`:

```yaml
---
# VersionMark Configuration File
# This file defines which tools to capture and how to extract their version information.

tools:
  # .NET SDK
  # Note: .NET SDK versions include up to 4 components (e.g., 10.0.102, 8.0.404)
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+(?:\.\d+)?)'

  # Git
  git:
    command: git --version
    regex: '(?i)git version (?<version>\d+\.\d+\.\d+)'

  # Node.js
  node:
    command: node --version
    regex: '(?i)v(?<version>\d+\.\d+\.\d+)'

  # npm
  npm:
    command: npm --version
    regex: '(?<version>\d+\.\d+\.\d+)'

  # SonarScanner for .NET (from dotnet tool list)
  dotnet-sonarscanner:
    command: dotnet tool list
    regex: '(?i)dotnet-sonarscanner\s+(?<version>\d+\.\d+\.\d+)'

  # Pandoc (DemaConsulting.PandocTool from dotnet tool list)
  pandoc:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.pandoctool\s+(?<version>\d+\.\d+\.\d+)'

  # WeasyPrint (DemaConsulting.WeasyPrintTool from dotnet tool list)
  weasyprint:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.weasyprinttool\s+(?<version>\d+\.\d+\.\d+)'

  # SarifMark (DemaConsulting.SarifMark from dotnet tool list)
  sarifmark:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.sarifmark\s+(?<version>\d+\.\d+\.\d+)'

  # SonarMark - checks global installation since it installs itself globally
  # Note: Different from template - uses --global flag
  sonarmark:
    command: dotnet tool list --global
    regex: '(?i)demaconsulting\.sonarmark\s+(?<version>\d+\.\d+\.\d+)'

  # ReqStream (DemaConsulting.ReqStream from dotnet tool list)
  reqstream:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.reqstream\s+(?<version>\d+\.\d+\.\d+)'

  # BuildMark (DemaConsulting.BuildMark from dotnet tool list)
  buildmark:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.buildmark\s+(?<version>\d+\.\d+\.\d+)'

  # VersionMark (DemaConsulting.VersionMark from dotnet tool list)
  versionmark:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.versionmark\s+(?<version>\d+\.\d+\.\d+)'
```

### 3.2 Update `.config/dotnet-tools.json`

**File:** `.config/dotnet-tools.json`

**Action:** Add VersionMark tool entry.

**Current State:**
```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "dotnet-sonarscanner": { "version": "11.1.0", "commands": ["dotnet-sonarscanner"] },
    "demaconsulting.spdxtool": { "version": "2.6.0", "commands": ["spdx-tool"] },
    "demaconsulting.pandoctool": { "version": "3.9.0", "commands": ["pandoc"] },
    "demaconsulting.weasyprinttool": { "version": "68.1.0", "commands": ["weasyprint"] },
    "demaconsulting.reqstream": { "version": "1.1.0", "commands": ["reqstream"] },
    "demaconsulting.sarifmark": { "version": "1.1.0", "commands": ["sarifmark"] },
    "demaconsulting.buildmark": { "version": "0.2.0", "commands": ["buildmark"] }
  }
}
```

**Required Change:** Add after buildmark:
```json
    "demaconsulting.versionmark": {
      "version": "0.1.0",
      "commands": [
        "versionmark"
      ]
    }
```

**Also Update:** Consider updating `demaconsulting.reqstream` from "1.1.0" to "1.2.0" (template has 1.2.0)

### 3.3 Update `.github/workflows/build.yaml` - Add Version Capture Steps

#### Job: quality-checks

**Insert after "Setup dotnet" step (after line 22):**

```yaml
      - name: Restore Tools
        run: >
          dotnet tool restore

      - name: Capture tool versions
        shell: bash
        run: |
          echo "Capturing tool versions..."
          dotnet versionmark --capture --job-id "quality" -- dotnet git versionmark
          echo "✓ Tool versions captured"

      - name: Upload version capture
        uses: actions/upload-artifact@v6
        with:
          name: version-capture-quality
          path: versionmark-quality.json
```

#### Job: build

**Insert after "Create Dotnet Tool" step (after line 119):**

```yaml
      - name: Capture tool versions
        shell: bash
        run: |
          echo "Capturing tool versions..."
          # Create short job ID: build-win, build-ubuntu
          OS_SHORT=$(echo "${{ matrix.os }}" | sed 's/windows-latest/win/;s/ubuntu-latest/ubuntu/')
          JOB_ID="build-${OS_SHORT}"
          dotnet versionmark --capture --job-id "${JOB_ID}" -- \
            dotnet git dotnet-sonarscanner versionmark
          echo "✓ Tool versions captured"

      - name: Upload version capture
        uses: actions/upload-artifact@v6
        with:
          name: version-capture-${{ matrix.os }}
          path: versionmark-build-*.json
```

#### Job: integration-test

**Insert after "Download package" step, add sparse checkout for .versionmark.yaml:**

**Replace line 204-208:**
```yaml
      - name: Download package
        uses: actions/download-artifact@v7
        with:
          name: artifacts-${{ matrix.os }}
          path: packages
```

**With:**
```yaml
      - name: Checkout
        uses: actions/checkout@v6
        with:
          sparse-checkout: |
            .versionmark.yaml
            .config/dotnet-tools.json

      - name: Download package
        uses: actions/download-artifact@v7
        with:
          name: artifacts-${{ matrix.os }}
          path: packages
```

**Then add after line 213 (after "Setup dotnet"):**

```yaml
      - name: Restore Tools
        run: >
          dotnet tool restore
```

**Then insert after "Run SonarMark self-validation" step (after line 270):**

```yaml
      - name: Capture tool versions
        shell: bash
        run: |
          echo "Capturing tool versions..."
          # Create short job ID: int-win-8, int-win-9, int-ubuntu-8, etc.
          OS_SHORT=$(echo "${{ matrix.os }}" | sed 's/windows-latest/win/;s/ubuntu-latest/ubuntu/')
          DOTNET_SHORT=$(echo "${{ matrix.dotnet-version }}" | sed 's/\.x$//')
          JOB_ID="int-${OS_SHORT}-${DOTNET_SHORT}"
          dotnet versionmark --capture --job-id "${JOB_ID}" -- dotnet git versionmark
          echo "✓ Tool versions captured"

      - name: Upload version capture
        uses: actions/upload-artifact@v6
        with:
          name: version-capture-${{ matrix.os }}-dotnet${{ matrix.dotnet-version }}
          path: versionmark-int-*.json
```

#### Job: build-docs

**Insert after "Download CodeQL SARIF" step (after line 313):**

```yaml
      - name: Download all version captures
        uses: actions/download-artifact@v7
        with:
          path: version-captures
          pattern: 'version-capture-*'
        continue-on-error: true
```

**Insert after "Restore Tools" step (after line 333):**

```yaml
      - name: Capture tool versions for build-docs
        shell: bash
        run: |
          echo "Capturing tool versions..."
          dotnet versionmark --capture --job-id "build-docs" -- \
            dotnet git node npm pandoc weasyprint sarifmark reqstream buildmark versionmark
          echo "✓ Tool versions captured"
```

**Note:** SonarMark is installed globally, so it won't appear in `dotnet tool list` for build-docs. That's OK - it will be captured in integration-test jobs.

**Insert after "Display Build Notes Report" step (after line 463):**

```yaml
      - name: Publish Tool Versions
        shell: bash
        run: |
          echo "Publishing tool versions..."
          dotnet versionmark --publish --report docs/buildnotes/versions.md --report-depth 1 \
            -- "versionmark-*.json" "version-captures/**/versionmark-*.json"
          echo "✓ Tool versions published"

      - name: Display Tool Versions Report
        shell: bash
        run: |
          echo "=== Tool Versions Report ==="
          cat docs/buildnotes/versions.md
```

### 3.4 Create `docs/buildnotes/versions.md` (Initial Placeholder)

**File:** `docs/buildnotes/versions.md`

**Action:** Create placeholder file (will be auto-generated by workflow).

```markdown
# Tool Versions

This document is auto-generated by VersionMark during the build process.
```

### 3.5 Add VersionMark to `.cspell.json`

**File:** `.cspell.json`

**Action:** Add "versionmark" and "VersionMark" to the words list if not already present.

---

## 4. Other Template Differences (Lower Priority)

### 4.1 Missing `.vscode` Directory Structure

The template includes `.vscode/` configuration. This is optional for SonarMark.

### 4.2 Different Tool Versions

| Tool | Template | SonarMark | Action |
|------|----------|-----------|--------|
| reqstream | 1.2.0 | 1.1.0 | Consider updating |
| versionmark | 0.1.0 | (missing) | **Required** |
| sonarmark | 1.1.0 | (self) | N/A |
| spdxtool | (not in template) | 2.6.0 | SonarMark-specific, keep |

---

## 5. Implementation Checklist

### Phase 1: Configuration Files (Required)

- [ ] Create `.versionmark.yaml` with SonarMark-specific configuration
- [ ] Update `.config/dotnet-tools.json` to add VersionMark 0.1.0
- [ ] Update `.config/dotnet-tools.json` to update ReqStream to 1.2.0 (optional)
- [ ] Create `docs/buildnotes/versions.md` placeholder
- [ ] Update `.cspell.json` to include VersionMark terms

### Phase 2: Workflow Updates (Required)

- [ ] Update `quality-checks` job: Add tool restore, version capture, upload steps
- [ ] Update `build` job: Add version capture and upload steps
- [ ] Update `integration-test` job: Add checkout (sparse), tool restore, version capture, upload steps
- [ ] Update `build-docs` job: Add version capture download, capture for build-docs, publish, display steps

### Phase 3: Testing (Required)

- [ ] Run local build to verify VersionMark installation
- [ ] Test version capture locally: `dotnet versionmark --capture --job-id "test" -- dotnet git`
- [ ] Trigger CI build to verify workflow changes
- [ ] Verify `docs/buildnotes/versions.md` is generated correctly
- [ ] Check build logs for version capture/publish output

### Phase 4: Documentation Updates (Optional)

- [ ] Update README.md to mention VersionMark if relevant
- [ ] Update build documentation to explain version tracking

---

## 6. How BuildMark Does It (Reference)

BuildMark follows the **exact same pattern** as the template:

1. **Global Installation:** BuildMark installs itself globally during integration-test and build-docs
2. **Special Configuration:** In `.versionmark.yaml`, BuildMark uses `dotnet tool list --global`:
   ```yaml
   buildmark:
     command: dotnet tool list --global
     regex: '(?i)demaconsulting\.buildmark\s+(?<version>\d+\.\d+\.\d+)'
   ```
3. **Version Capture:** Each job captures versions and uploads artifacts
4. **Version Publish:** build-docs job aggregates and publishes the version report

**SonarMark should follow the identical pattern.**

---

## 7. Benefits of Tool Version Tracking

1. **Build Transparency:** Complete record of all tool versions used in the build
2. **Reproducibility:** Enables exact reconstruction of build environment
3. **Debugging:** Helps diagnose tool-related build issues
4. **Compliance:** Provides audit trail for regulated environments
5. **Documentation:** Auto-generates version manifest in build notes

---

## 8. Summary

### Critical Changes (Must Implement)
1. ✅ Global installation pattern (already correct)
2. ❌ Add `.versionmark.yaml` with SonarMark-specific configuration
3. ❌ Add VersionMark to `.config/dotnet-tools.json`
4. ❌ Add version capture/upload steps to all workflow jobs
5. ❌ Add version publish step to build-docs job

### Optional Changes
- Update ReqStream from 1.1.0 to 1.2.0
- Add .vscode configuration (if desired)

### Next Steps
1. Create `.versionmark.yaml` configuration file
2. Update `.config/dotnet-tools.json`
3. Update `.github/workflows/build.yaml` with version capture steps
4. Test locally with `dotnet tool restore` and `dotnet versionmark`
5. Commit and push changes
6. Verify CI build generates version report

---

**END OF ANALYSIS**
