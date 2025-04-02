# ⚙️ MetaMeta CI/CD Pipeline Prompt (GitHub Actions)

## 🎯 Objective
Generate a **GitHub Actions YAML build pipeline** for the MetaMeta project that:

- Restores and builds all projects in the solution
- Runs all test projects (unit + agent tests)
- Publishes test results and code coverage
- Caches NuGet packages
- Uses .NET 9 SDK and Ubuntu runner
- Supports both PR and `main` branch triggers

---

## 🧱 Expected Project Structure

```
/
├── MetaMeta.sln
├── src/
│   ├── MetaMeta.ApiService/
│   ├── MetaMeta.AppHost/
│   ├── MetaMeta.Core/
│   ├── MetaMeta.Orchestration/
│   └── ...
├── tests/
│   ├── MetaMeta.Core.Tests/
│   └── MetaMeta.AgentTests/
```

---

## 📁 Target Output File

Generate the CI config to:
```
.github/workflows/build.yml
```

---

## 🔧 Build Steps

1. **Checkout**
2. **Setup .NET 9 SDK**
3. **Restore**
4. **Build (Release mode)**
5. **Run Tests**
6. **Publish results**
7. **Upload artifacts**

---

## 🧪 Output Format

```yaml
# .github/workflows/build.yml
name: Build and Test MetaMeta

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - name: Checkout Repo
      uses: actions/checkout@v4

    - name: Setup .NET 9 SDK
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 9.0.x

    - name: Restore dependencies
      run: dotnet restore MetaMeta.sln

    - name: Build
      run: dotnet build MetaMeta.sln --configuration Release --no-restore

    - name: Run Tests
      run: dotnet test MetaMeta.sln --configuration Release --no-build --logger "trx"

    - name: Upload Test Results
      if: always()
      uses: actions/upload-artifact@v4
      with:
        name: test-results
        path: '**/TestResults/*.trx'

    - name: Cache NuGet Packages
      uses: actions/cache@v3
      with:
        path: ~/.nuget/packages
        key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
        restore-keys: |
          ${{ runner.os }}-nuget-
```

---

## 🛠 Execution Instructions

1. Save as `.cursor/016_ci-build-pipeline.prompt.md`
2. Run in Cursor → "Run Prompt on Workspace"
3. Result is created at `.github/workflows/build.yml`
4. Commit and push to GitHub to activate

---

## 🛡 Bonus Protection

Want me to:
- Add `build.yml` validation to `.mdc` rules?
- Create `017_release-pipeline.prompt.md` for tagged deploys?
- Add matrix builds for multi-framework targets?

Just say "Ship it," and I'll stack your pipeline to the moon. 🌕 