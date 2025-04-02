# Versioning Guidelines for MetaMeta AI IDE

This project follows [Semantic Versioning 2.0.0](https://semver.org/) for version numbering.

## Version Format

Versions follow the format of `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILD]`

- **MAJOR**: Incremented for incompatible API changes
- **MINOR**: Incremented for backwards-compatible new functionality
- **PATCH**: Incremented for backwards-compatible bug fixes
- **PRERELEASE**: Optional label for pre-release versions (alpha, beta, rc)
- **BUILD**: Optional build metadata

## Branching Strategy

We use a modified GitFlow workflow:

- `main`: Production-ready code, tagged with release versions
- `develop`: Integration branch for features
- `feature/*`: New features and improvements
- `release/*`: Release preparation branches
- `hotfix/*`: Emergency fixes for production releases

## Release Process

1. Feature development occurs in `feature/*` branches
2. Features are merged into `develop` when complete
3. When ready for release, create a `release/x.y.z` branch from `develop`
4. After testing, merge `release/x.y.z` into both `main` and `develop`
5. Tag the `main` branch with the version number
6. For emergency fixes, create a `hotfix/*` branch from `main`, then merge back to both `main` and `develop`

## Version File

The current version is maintained in the `VERSION` file at the repository root.

## Versioning Tools

For .NET projects, we maintain version numbers in:
- `VERSION` file (source of truth)
- `.csproj` files via `<Version>` tag
- Assembly info via attributes
- Docker image tags 