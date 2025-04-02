# 🚀 MetaMeta Release Pipeline Prompt (GitHub Actions)

## 🎯 Objective
Generate a **GitHub Actions YAML release pipeline** for the MetaMeta project that:

- Triggers on release tags (v*.*.*)
- Builds and tests the solution
- Creates GitHub releases
- Builds and publishes Docker containers
- Pushes containers to container registry
- Generates release notes from commits

---

## 📁 Target Output File

Generate the release config to:
```
.github/workflows/release.yml
```

---

## 🔧 Release Steps

1. **Checkout code** with full git history
2. **Setup .NET 9 SDK**
3. **Build and test** the solution
4. **Extract version** from tag
5. **Build Docker images**
6. **Push to container registry**
7. **Create GitHub release**
8. **Generate release notes**

---

## 🧪 Output Format

```yaml
# .github/workflows/release.yml
name: Release MetaMeta

on:
  push:
    tags:
      - 'v*.*.*'

jobs:
  build-and-test:
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

      - name: Run tests
        run: dotnet test MetaMeta.sln --configuration Release --no-build --verbosity normal

  create-release:
    needs: build-and-test
    runs-on: ubuntu-latest
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Extract version from tag
        id: extract_version
        run: echo "VERSION=${GITHUB_REF#refs/tags/v}" >> $GITHUB_OUTPUT

      - name: Create GitHub Release
        id: create_release
        uses: actions/create-release@v1
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          tag_name: ${{ github.ref }}
          release_name: Release ${{ github.ref_name }}
          draft: false
          prerelease: false
          body: |
            Release ${{ github.ref_name }} of MetaMeta
            
            See [CHANGELOG.md](https://github.com/${{ github.repository }}/blob/main/CHANGELOG.md) for details.

  build-and-push-containers:
    needs: create-release
    runs-on: ubuntu-latest
    strategy:
      matrix:
        service: [ApiService, GrpcService]
    
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Extract version from tag
        id: extract_version
        run: echo "VERSION=${GITHUB_REF#refs/tags/v}" >> $GITHUB_OUTPUT

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v2

      - name: Login to GitHub Container Registry
        uses: docker/login-action@v2
        with:
          registry: ghcr.io
          username: ${{ github.repository_owner }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Build and push ${{ matrix.service }} container
        uses: docker/build-push-action@v4
        with:
          context: .
          file: src/MetaMeta.${{ matrix.service }}/Dockerfile
          push: true
          tags: |
            ghcr.io/${{ github.repository_owner }}/metameta-${{ matrix.service }}:latest
            ghcr.io/${{ github.repository_owner }}/metameta-${{ matrix.service }}:${{ steps.extract_version.outputs.VERSION }}
          cache-from: type=gha
          cache-to: type=gha,mode=max
```

---

## 🛠 Execution Instructions

1. Save as `.cursor/017_release-pipeline.prompt.md`
2. Run in Cursor → "Run Prompt on Workspace"
3. Result is created at `.github/workflows/release.yml`
4. Ensure you have a Dockerfile in each service project
5. Commit and push to GitHub to activate

---

## 📋 Prerequisites

For this workflow to work properly:

1. **Container Registry Access**: Ensure your GitHub Actions have write access to GitHub Container Registry (GHCR)
2. **Dockerfiles**: Create Dockerfiles in service project directories
3. **Semantic Versioning**: Use tags in format `v1.2.3` for releases
4. **CHANGELOG.md**: Maintain a changelog in your repository root 