#!/bin/bash
# ./scripts/extract-package-versions-manual.sh

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
  sed 's/<PackageReference Include="\([^"]*\)" Version="\([^"]*\)"/\1|\2/g' >> "$TEMP_FILE"
done

# Sort and remove duplicates
sort -u "$TEMP_FILE" > "${TEMP_FILE}.sorted"

# Format for Directory.Packages.props
echo -e "\n--- Copy these entries to Directory.Packages.props ---\n"
while IFS= read -r LINE; do
  PACKAGE=$(echo "$LINE" | cut -d'|' -f1)
  VERSION=$(echo "$LINE" | cut -d'|' -f2)
  echo "    <PackageVersion Include=\"$PACKAGE\" Version=\"$VERSION\" />"
done < "${TEMP_FILE}.sorted"
echo -e "\n--- End of entries ---\n"

# Clean up
rm "$TEMP_FILE" "${TEMP_FILE}.sorted"

echo -e "✅ Package version extraction complete." 