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
3. **Cache NuGet packages**
4. **Restore**
5. **Build (Release mode)**
6. **Run Tests with coverage**
7. **Publish test results and coverage reports**
8. **Upload artifacts**

---

## 🧪 Output Format

```yaml
# .github/workflows/build.yml
name: Build and Test MetaMeta

on:
  push:
    branches: [ main, release/*, feature/* ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - name: Checkout repository
      uses: actions/checkout@v4
      with:
        fetch-depth: 0

    - name: Setup .NET 9 SDK
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'

    - name: Cache NuGet packages
      uses: actions/cache@v3
      with:
        path: ~/.nuget/packages
        key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
        restore-keys: |
          ${{ runner.os }}-nuget-

    - name: Restore dependencies
      run: dotnet restore MetaMeta.sln

    - name: Build
      run: dotnet build MetaMeta.sln --configuration Release --no-restore

    - name: Run tests with coverage
      run: dotnet test MetaMeta.sln --configuration Release --no-build --logger "trx;LogFileName=testresults.trx" --collect:"XPlat Code Coverage" --results-directory ./TestResults

    - name: Upload test results
      if: always()
      uses: actions/upload-artifact@v4
      with:
        name: test-results
        path: TestResults
        retention-days: 7

    - name: Generate code coverage report
      if: success()
      uses: danielpalme/ReportGenerator-GitHub-Action@5.2.0
      with:
        reports: 'TestResults/**/coverage.cobertura.xml'
        targetdir: 'CoverageReport'
        reporttypes: 'HtmlInline_AzurePipelines;Cobertura;Badges'
        verbosity: 'Info'

    - name: Upload coverage report
      if: success()
      uses: actions/upload-artifact@v4
      with:
        name: coverage-report
        path: CoverageReport
        retention-days: 7

    - name: Publish ApiService
      if: github.ref == 'refs/heads/main'
      run: dotnet publish src/MetaMeta.ApiService/MetaMeta.ApiService.csproj -c Release -o publish/api --no-restore

    - name: Publish GrpcService
      if: github.ref == 'refs/heads/main'
      run: dotnet publish src/MetaMeta.GrpcService/MetaMeta.GrpcService.csproj -c Release -o publish/grpc --no-restore

    - name: Upload API artifacts
      if: github.ref == 'refs/heads/main'
      uses: actions/upload-artifact@v4
      with:
        name: api-artifacts
        path: publish/api
        retention-days: 7

    - name: Upload gRPC artifacts
      if: github.ref == 'refs/heads/main'
      uses: actions/upload-artifact@v4
      with:
        name: grpc-artifacts
        path: publish/grpc
        retention-days: 7
```

---

## 🛠 Execution Instructions

1. Save as `.cursor/016_ci-build-pipeline.prompt.md`
2. Run in Cursor → "Run Prompt on Workspace"
3. Result is created at `.github/workflows/build.yml`
4. Commit and push to GitHub to activate

---

## 🛡 Advanced Features

Here are additional CI/CD improvements you can request:

- **Matrix builds** for multi-framework targets (.NET 8/9)
- **Release pipeline** for tagged deploys (semantic versioning)
- **Docker container** builds for microservices
- **Integration with Azure** for deployment to AKS/App Service
- **Code quality analysis** with SonarCloud or CodeQL
- **Security scanning** for NuGet dependencies 