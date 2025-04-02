# 🔒 MetaMeta Security Scanning Prompt (GitHub Actions)

## 🎯 Objective
Generate a **GitHub Actions YAML workflow** for the MetaMeta project that:

- Scans code for security vulnerabilities
- Checks for outdated NuGet packages with known CVEs
- Uses tools like CodeQL or SonarCloud
- Provides security report artifacts
- Runs on a schedule and on pull requests

---

## 📁 Target Output File

Generate the security config to:
```
.github/workflows/security.yml
```

---

## 🔧 Security Scanning Steps

1. **Checkout code**
2. **Initialize CodeQL**
3. **Build code** for analysis
4. **Perform CodeQL analysis**
5. **Run NuGet vulnerability scanner**
6. **Generate security report**
7. **Upload security artifacts**

---

## 🧪 Output Format

```yaml
# .github/workflows/security.yml
name: Security Scanning

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]
  schedule:
    - cron: '0 0 * * 0'  # Run weekly on Sundays at midnight

jobs:
  analyze:
    name: CodeQL Analysis
    runs-on: ubuntu-latest
    permissions:
      security-events: write
      actions: read
      contents: read

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Initialize CodeQL
        uses: github/codeql-action/init@v2
        with:
          languages: csharp

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'

      - name: Autobuild
        uses: github/codeql-action/autobuild@v2

      - name: Perform CodeQL Analysis
        uses: github/codeql-action/analyze@v2
        with:
          category: "/language:csharp"

  dependency-check:
    name: NuGet Dependency Check
    runs-on: ubuntu-latest
    permissions:
      contents: read
      security-events: write

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'

      - name: Install dotnet-outdated
        run: dotnet tool install --global dotnet-outdated-tool

      - name: Check Outdated Packages
        run: dotnet outdated --version-lock Major --include-transitive -o outdated-packages.json -f json

      - name: Install OWASP Dependency Check
        run: |
          mkdir -p $HOME/tools
          wget -q https://github.com/jeremylong/DependencyCheck/releases/download/v7.4.4/dependency-check-7.4.4-release.zip
          unzip -q dependency-check-7.4.4-release.zip -d $HOME/tools
          rm dependency-check-7.4.4-release.zip

      - name: Run OWASP Dependency Check
        run: |
          $HOME/tools/dependency-check/bin/dependency-check.sh --scan . --project "MetaMeta" --out . --format "ALL"

      - name: Upload Dependency Check Results
        uses: actions/upload-artifact@v4
        with:
          name: dependency-check-report
          path: |
            dependency-check-report.html
            dependency-check-report.xml
            dependency-check-report.json
            outdated-packages.json
          retention-days: 7

      - name: Convert to SARIF
        run: |
          pip install defusedxml
          curl -sSL https://raw.githubusercontent.com/Sjord/owasp-dependency-check-to-sarif/master/convert.py | python - dependency-check-report.xml > dependency-check-results.sarif

      - name: Upload SARIF Report
        uses: github/codeql-action/upload-sarif@v2
        with:
          sarif_file: dependency-check-results.sarif
          category: owasp-dependency-check
```

---

## 🛠 Execution Instructions

1. Save as `.cursor/018_security-scanning.prompt.md`
2. Run in Cursor → "Run Prompt on Workspace"
3. Result is created at `.github/workflows/security.yml`
4. Commit and push to GitHub to activate

---

## 📋 Prerequisites

For this workflow to work properly:

1. **GitHub CodeQL**: Ensure CodeQL is enabled for your repository
2. **Permissions**: The workflow needs security-events write permission
3. **Regular Runs**: The schedule ensures weekly checks, but you can adjust the cron schedule as needed 