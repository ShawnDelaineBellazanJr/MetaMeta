#!/bin/bash
# ./scripts/extract-package-versions.sh

echo -e "\n🔍 Extracting package versions from backup files"

# Create a temporary file to store the package versions
TEMP_FILE=$(mktemp)

# Find all backup project files
BACKUP_FILES=$(find src tests -name "*.csproj.bak")

# Extract package references with versions
for BACKUP_FILE in $BACKUP_FILES; do
  echo "Processing: $BACKUP_FILE"
  
  # Extract PackageReference lines with versions
  grep -o '<PackageReference Include="[^"]*" Version="[^"]*"' "$BACKUP_FILE" | \
  sed 's/<PackageReference Include="\([^"]*\)" Version="\([^"]*\)"/    <PackageVersion Include="\1" Version="\2" \/>/g' >> "$TEMP_FILE"
done

# Sort and remove duplicates
sort -u "$TEMP_FILE" > "${TEMP_FILE}.sorted"

# Merge with existing Directory.Packages.props
if [ -f "Directory.Packages.props" ]; then
  # Extract the existing ItemGroup content
  EXISTING_VERSIONS=$(grep -o '<PackageVersion Include="[^"]*" Version="[^"]*"' "Directory.Packages.props" | \
                      sed 's/<PackageVersion Include="\([^"]*\)" Version="\([^"]*\)"/\1|\2/g')
  
  # Extract the new package versions, excluding ones already in the file
  NEW_VERSIONS=""
  while IFS= read -r LINE; do
    PACKAGE=$(echo "$LINE" | grep -o 'Include="[^"]*"' | cut -d'"' -f2)
    if ! echo "$EXISTING_VERSIONS" | grep -q "^$PACKAGE|"; then
      NEW_VERSIONS="$NEW_VERSIONS$LINE
"
    fi
  done < "${TEMP_FILE}.sorted"
  
  # Add new versions to the file, just before the closing ItemGroup tag
  if [ -n "$NEW_VERSIONS" ]; then
    # Create a temporary file with the new content
    cp "Directory.Packages.props" "${TEMP_FILE}.new"
    sed -i "s|</ItemGroup>|$NEW_VERSIONS</ItemGroup>|" "${TEMP_FILE}.new"
    mv "${TEMP_FILE}.new" "Directory.Packages.props"
    echo "✅ Added new package versions to Directory.Packages.props"
  else
    echo "ℹ️ No new package versions to add"
  fi
else
  echo "❌ Directory.Packages.props file not found"
fi

# Clean up
rm "$TEMP_FILE" "${TEMP_FILE}.sorted"

echo -e "\n✅ Package version extraction complete." 