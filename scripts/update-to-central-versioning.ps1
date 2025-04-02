#!/usr/bin/env pwsh
# ./scripts/update-to-central-versioning.ps1

Write-Host "`n🔄 Updating projects to use central package versioning" -ForegroundColor Cyan

$projectFiles = Get-ChildItem -Path "src", "tests" -Recurse -Filter "*.csproj"
$count = 0

foreach ($projectFile in $projectFiles) {
    Write-Host "Processing: $($projectFile.FullName)" -ForegroundColor Yellow
    
    # Load the project file content
    [xml]$projectXml = Get-Content $projectFile.FullName
    
    # Check if central package management is already enabled
    $propertyGroup = $projectXml.Project.PropertyGroup
    $alreadyEnabled = $false
    
    if ($propertyGroup -ne $null) {
        if ($propertyGroup.GetType().Name -eq "Object[]") {
            foreach ($pg in $propertyGroup) {
                if ($pg.CentralPackageManagementEnabled -eq "true") {
                    $alreadyEnabled = $true
                    break
                }
            }
        } else {
            if ($propertyGroup.CentralPackageManagementEnabled -eq "true") {
                $alreadyEnabled = $true
            }
        }
    }
    
    # Add central package management property if not already enabled
    if (-not $alreadyEnabled) {
        $modified = $false
        
        # Find all package references with explicit versions
        $itemGroups = $projectXml.Project.ItemGroup
        
        if ($itemGroups -ne $null) {
            # Handle array or single item group
            if ($itemGroups.GetType().Name -eq "Object[]") {
                foreach ($itemGroup in $itemGroups) {
                    $packageRefs = $itemGroup.PackageReference
                    if ($packageRefs -ne $null) {
                        if ($packageRefs.GetType().Name -eq "Object[]") {
                            foreach ($packageRef in $packageRefs) {
                                if ($packageRef.Version -ne $null) {
                                    # Remove version attribute
                                    $packageRef.RemoveAttribute("Version")
                                    $modified = $true
                                }
                            }
                        } else {
                            if ($packageRefs.Version -ne $null) {
                                # Remove version attribute
                                $packageRefs.RemoveAttribute("Version")
                                $modified = $true
                            }
                        }
                    }
                }
            } else {
                $packageRefs = $itemGroups.PackageReference
                if ($packageRefs -ne $null) {
                    if ($packageRefs.GetType().Name -eq "Object[]") {
                        foreach ($packageRef in $packageRefs) {
                            if ($packageRef.Version -ne $null) {
                                # Remove version attribute
                                $packageRef.RemoveAttribute("Version")
                                $modified = $true
                            }
                        }
                    } else {
                        if ($packageRefs.Version -ne $null) {
                            # Remove version attribute
                            $packageRefs.RemoveAttribute("Version")
                            $modified = $true
                        }
                    }
                }
            }
        }
        
        # Add the central package management property
        $firstPropertyGroup = $null
        if ($propertyGroup -ne $null) {
            if ($propertyGroup.GetType().Name -eq "Object[]") {
                $firstPropertyGroup = $propertyGroup[0]
            } else {
                $firstPropertyGroup = $propertyGroup
            }
        } else {
            # Create a new PropertyGroup if none exists
            $firstPropertyGroup = $projectXml.CreateElement("PropertyGroup", $projectXml.Project.NamespaceURI)
            $projectXml.Project.PrependChild($firstPropertyGroup)
        }
        
        # Add the property
        $centralPropElement = $projectXml.CreateElement("ManagePackageVersionsCentrally", $projectXml.Project.NamespaceURI)
        $centralPropElement.InnerText = "true"
        $firstPropertyGroup.AppendChild($centralPropElement)
        $modified = $true
        
        # Save if modified
        if ($modified) {
            $projectXml.Save($projectFile.FullName)
            Write-Host "✅ Updated: $($projectFile.Name)" -ForegroundColor Green
            $count++
        } else {
            Write-Host "ℹ️ No changes needed for: $($projectFile.Name)" -ForegroundColor Blue
        }
    } else {
        Write-Host "ℹ️ Already using central versioning: $($projectFile.Name)" -ForegroundColor Blue
    }
}

Write-Host "`n✅ Completed! Updated $count project files to use central package versioning." -ForegroundColor Green 