#!/bin/bash
# ./scripts/update-to-central-versioning-improved.sh

echo -e "\n🔄 Updating projects to use central package versioning"

# Find all project files
PROJECT_FILES=$(find src tests -name "*.csproj")
COUNT=0

for PROJECT_FILE in $PROJECT_FILES; do
  echo "Processing: $PROJECT_FILE"
  
  # Create a backup of the original file
  cp "$PROJECT_FILE" "${PROJECT_FILE}.bak"
  
  # Add ManagePackageVersionsCentrally property to the first PropertyGroup if not exists
  if ! grep -q "<ManagePackageVersionsCentrally>" "$PROJECT_FILE"; then
    # Insert the property right after the first PropertyGroup opening
    sed -i '/<PropertyGroup>/a \    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>' "$PROJECT_FILE"
    echo "✅ Added central versioning property"
  fi
  
  # Check if any version attributes were removed
  VERSIONS_FOUND=$(grep -c 'Version="' "$PROJECT_FILE")
  
  # Remove Version attributes from PackageReference elements
  sed -i 's/<PackageReference Include="\([^"]*\)" Version="[^"]*"/<PackageReference Include="\1"/g' "$PROJECT_FILE"
  
  # Check if any versions were actually removed
  VERSIONS_AFTER=$(grep -c 'Version="' "$PROJECT_FILE")
  
  if [ $VERSIONS_AFTER -lt $VERSIONS_FOUND ]; then
    echo "✅ Removed $(($VERSIONS_FOUND - $VERSIONS_AFTER)) version attributes"
    COUNT=$((COUNT+1))
  else
    echo "ℹ️ No version attributes removed"
  fi
done

echo -e "\n✅ Completed! Updated $COUNT project files to use central package versioning." 