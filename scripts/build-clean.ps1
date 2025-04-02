# ./scripts/build-clean.ps1 - Enhanced with conflict detection and security features
param(
    [switch]$Restore = $true,
    [switch]$Build = $true,
    [switch]$DetectConflicts = $true,
    [switch]$VerifyIntegrity = $false
)

Write-Host "`n🧽 INITIATING: MetaMeta Build Cleanse Protocol (v1.1.0)" -ForegroundColor Cyan

$srcPath = "$PSScriptRoot/../src"
$testsPath = "$PSScriptRoot/../tests"

function Clean-Output {
    param([string]$path)
    Write-Host "🔍 Cleaning build artifacts in $path" -ForegroundColor Yellow
    Get-ChildItem -Path $path -Include bin,obj -Recurse -Force | Remove-Item -Force -Recurse -ErrorAction SilentlyContinue
    Write-Host "✅ Cleaned: $path" -ForegroundColor Green
}

function Detect-PackageConflicts {
    if ($DetectConflicts) {
        Write-Host "`n🔍 Scanning for package conflicts..." -ForegroundColor Yellow
        # Find all .csproj files
        $projectFiles = Get-ChildItem -Path $srcPath,$testsPath -Include "*.csproj" -Recurse
        
        # Check if Directory.Packages.props exists
        $packagesPropsPath = "$PSScriptRoot/../Directory.Packages.props"
        if (Test-Path $packagesPropsPath) {
            Write-Host "✅ Using central package management" -ForegroundColor Green
            
            # Check for any project not using central package management
            $nonCpmProjects = 0
            foreach ($projectFile in $projectFiles) {
                $content = Get-Content -Path $projectFile.FullName
                if ($content -notmatch "<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>") {
                    Write-Host "⚠️ Project not using central package management: $($projectFile.FullName)" -ForegroundColor Yellow
                    $nonCpmProjects++
                }
            }
            
            if ($nonCpmProjects -gt 0) {
                Write-Host "⚠️ Found $nonCpmProjects projects not using central package management" -ForegroundColor Yellow
                Write-Host "   Consider running: ./scripts/update-to-central-versioning.ps1" -ForegroundColor Yellow
            } else {
                Write-Host "✅ All projects are using central package management" -ForegroundColor Green
            }
        } else {
            Write-Host "⚠️ Directory.Packages.props not found - central package management not configured" -ForegroundColor Yellow
        }
    }
}

function Check-PackageIntegrity {
    if ($VerifyIntegrity) {
        Write-Host "`n🔒 Verifying package integrity..." -ForegroundColor Yellow
        # This would validate downloaded packages against expected hashes
        # Requires additional setup with a hash verification mechanism
        Write-Host "⚠️ Package integrity verification not implemented in development mode" -ForegroundColor Yellow
    }
}

Write-Host "`n🔍 Locating all build outputs in /src and /tests..." -ForegroundColor Yellow
Clean-Output -path $srcPath
Clean-Output -path $testsPath

# Check for problematic files
$projectFiles = Get-ChildItem -Path $srcPath,$testsPath -Include "*.csproj" -Recurse
foreach ($project in $projectFiles) {
    $projectDir = $project.DirectoryName
    $assetsPath = Join-Path $projectDir "obj/project.assets.json"
    $nugetProps = Join-Path $projectDir "obj/project.nuget.cache"
    
    foreach ($file in @($assetsPath, $nugetProps)) {
        if (Test-Path $file) {
            Remove-Item -Path $file -Force
            Write-Host "🗑️ Deleted lock file: $file" -ForegroundColor Red
        }
    }
}

# Detect package conflicts
Detect-PackageConflicts

# Verify package integrity
Check-PackageIntegrity

if ($Restore) {
    Write-Host "`n🔄 Executing: dotnet restore with enhanced settings" -ForegroundColor Cyan
    # Use the retry pattern for resilience
    $maxRetries = 3
    $retryCount = 0
    $restoreSuccess = $false
    
    while ($retryCount -lt $maxRetries -and -not $restoreSuccess) {
        $retryCount++
        
        if ($retryCount -gt 1) {
            Write-Host "🔁 Retry attempt $retryCount of $maxRetries" -ForegroundColor Yellow
            # Wait a bit before retrying
            Start-Sleep -Seconds 2
        }
        
        # Use additional restore flags for better diagnostics
        $result = dotnet restore --force --verbosity minimal
        
        if ($LASTEXITCODE -eq 0) {
            $restoreSuccess = $true
            Write-Host "✅ Restore completed successfully" -ForegroundColor Green
        } else {
            Write-Host "⚠️ Restore failed, retrying..." -ForegroundColor Yellow
        }
    }
    
    if (-not $restoreSuccess) {
        Write-Host "❌ Restore failed after $maxRetries attempts" -ForegroundColor Red
        exit 1
    }
}

if ($Build) {
    Write-Host "`n🏗️ Executing: dotnet build with enhanced settings" -ForegroundColor Cyan
    # Use additional build flags
    $result = dotnet build "$PSScriptRoot/../MetaMeta.sln" --no-restore --verbosity minimal
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Build completed successfully" -ForegroundColor Green
    } else {
        Write-Host "❌ Build failed" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n✅ MetaMeta cleansing ritual complete." -ForegroundColor Green 