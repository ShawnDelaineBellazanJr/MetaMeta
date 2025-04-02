#!/bin/bash
# ./scripts/update-to-central-versioning.sh

echo -e "\n🔄 Updating projects to use central package versioning"

# Find all project files
PROJECT_FILES=$(find src tests -name "*.csproj")
COUNT=0

for PROJECT_FILE in $PROJECT_FILES; do
  echo "Processing: $PROJECT_FILE"
  
  # Check if central package management is already enabled
  if grep -q "<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>" "$PROJECT_FILE"; then
    echo "ℹ️ Already using central versioning: $PROJECT_FILE"
    continue
  fi
  
  # Create a temporary file
  TEMP_FILE=$(mktemp)
  
  # Process the file
  MODIFIED=0
  
  # Add ManagePackageVersionsCentrally property if not exists
  if ! grep -q "<ManagePackageVersionsCentrally>" "$PROJECT_FILE"; then
    # Add the property after the first PropertyGroup opening tag
    awk '
      BEGIN { added = 0 }
      /<PropertyGroup>/ && !added {
        print $0
        print "    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>"
        added = 1
        next
      }
      { print }
    ' "$PROJECT_FILE" > "$TEMP_FILE"
    
    if [ $? -eq 0 ]; then
      cp "$TEMP_FILE" "$PROJECT_FILE"
      MODIFIED=1
    fi
  fi
  
  # Remove Version attributes from PackageReference elements
  awk '
    BEGIN { modified = 0 }
    {
      if (match($0, /<PackageReference Include="([^"]+)" Version="[^"]+"/, arr)) {
        package = arr[1]
        modified = 1
        print "    <PackageReference Include=\"" package "\" />"
      } else {
        print $0
      }
    }
    END { exit modified }
  ' "$PROJECT_FILE" > "$TEMP_FILE"
  
  if [ $? -eq 1 ]; then
    cp "$TEMP_FILE" "$PROJECT_FILE"
    MODIFIED=1
  fi
  
  # Clean up
  rm "$TEMP_FILE"
  
  if [ $MODIFIED -eq 1 ]; then
    echo "✅ Updated: $PROJECT_FILE"
    COUNT=$((COUNT+1))
  else
    echo "ℹ️ No changes needed for: $PROJECT_FILE"
  fi
done

echo -e "\n✅ Completed! Updated $COUNT project files to use central package versioning." 