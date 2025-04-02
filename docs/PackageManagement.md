# Package Management System

This document describes the package management approach used in the MetaMeta project. It outlines how dependencies are managed, versioned, and maintained for consistency and security.

## Central Package Management

MetaMeta uses [Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management) to ensure all projects use consistent package versions. This approach:

- Eliminates version conflicts between projects
- Makes updating dependencies easier
- Simplifies security maintenance
- Reduces build errors

### Key Files

| File | Purpose |
|------|---------|
| `Directory.Packages.props` | Central list of all package versions |
| `Directory.Build.targets` | MSBuild targets that enforce versioning rules |
| `global.json` | Defines SDK version for consistency |

## How to Add a New Package

To add a new package to the project:

1. Add the package to the appropriate section in `Directory.Packages.props`
2. Reference the package without a version attribute in your project file:

```xml
<ItemGroup>
  <PackageReference Include="PackageName" />
</ItemGroup>
```

### Example of Adding a Package

```xml
<!-- In Directory.Packages.props -->
<ItemGroup>
  <PackageVersion Include="NewPackage" Version="1.2.3" />
</ItemGroup>

<!-- In YourProject.csproj -->
<ItemGroup>
  <PackageReference Include="NewPackage" />
</ItemGroup>
```

## Version Management

### Version Variables

We use MSBuild properties to group related packages under the same version. This makes it easier to update dependent packages together:

```xml
<PropertyGroup>
  <MSExtensionsVersion>10.0.0-preview.2.25163.2</MSExtensionsVersion>
</PropertyGroup>

<ItemGroup>
  <PackageVersion Include="Microsoft.Extensions.Configuration" Version="$(MSExtensionsVersion)" />
  <PackageVersion Include="Microsoft.Extensions.Logging" Version="$(MSExtensionsVersion)" />
</ItemGroup>
```

### Prerelease Packages

For prerelease packages, we use the `IsPrerelease` attribute to explicitly mark them:

```xml
<PackageVersion Include="Microsoft.SemanticKernel.Agents.Core" Version="1.44.0-preview" IsPrerelease="true" />
```

## Maintenance Scripts

Several maintenance scripts are available to help manage packages:

| Script | Purpose |
|--------|---------|
| `scripts/build-clean.sh` | Cleans build artifacts and restores packages |
| `scripts/update-to-central-versioning.sh` | Migrates projects to use central versioning |

### Using the Clean Script

```bash
# Basic usage
./scripts/build-clean.sh

# Skip build step
./scripts/build-clean.sh --no-build

# Skip restore step
./scripts/build-clean.sh --no-restore

# Skip conflict detection
./scripts/build-clean.sh --no-conflict-detection

# Enable package integrity verification (advanced)
./scripts/build-clean.sh --verify-integrity
```

## CI/CD Integration

The GitHub workflow in `.github/workflows/package-validation.yml` automatically validates:

- All projects are using central package management
- No project has explicit version attributes
- SDK version is consistent
- Package versions are consistent

## Security Best Practices

1. **Version Locking**: We lock versions using `CentralPackageTransitivePinningEnabled` to prevent unexpected updates
2. **Version Override Prevention**: We disable version overrides using `EnablePackageVersionOverride=false`
3. **Regular Updates**: Critical packages are updated regularly to address security vulnerabilities
4. **Prerelease Marking**: We explicitly mark prerelease packages to make them visible

## Troubleshooting

### Common Issues

#### Package Downgrade Errors

If you see a message like:
```
error NU1109: Detected package downgrade: Microsoft.Extensions.Configuration.Binder from X to Y
```

Solution: Ensure all package versions in `Directory.Packages.props` are consistent, especially for related packages.

#### Package Not Found

If you see a message like:
```
error NU1102: Unable to find package X with version Y
```

Solution: Check if the package exists in the specified version, particularly for preview packages.

### Running Clean Build

If you encounter persistent NuGet errors:

1. Run the clean script: `./scripts/build-clean.sh`
2. Check for projects not using central versioning: `./scripts/build-clean.sh --detect-conflicts`
3. Manually fix any detected issues
4. Restart the build process

## Future Improvements

- Hash verification for package integrity
- Automatic vulnerability scanning
- Dependency graph analysis 