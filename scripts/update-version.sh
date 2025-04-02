#!/bin/bash

# Script to update version numbers across the project
# Usage: ./update-version.sh [newversion]

set -e

# Ensure we're in the repository root
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR/.."

# Get the current version from VERSION file
CURRENT_VERSION=$(cat VERSION)

# If no version is provided, use the current version
if [ -z "$1" ]; then
  echo "Current version: $CURRENT_VERSION"
  echo "Usage: ./update-version.sh [newversion]"
  exit 0
fi

NEW_VERSION=$1

# Update the VERSION file
echo "$NEW_VERSION" > VERSION
echo "Updated VERSION file to $NEW_VERSION"

# Find all .csproj files and update the Version element
find src -name "*.csproj" | while read -r project_file; do
  if grep -q "<Version>" "$project_file"; then
    # Replace existing Version tag
    sed -i "s|<Version>.*</Version>|<Version>$NEW_VERSION</Version>|" "$project_file"
  else
    # Add Version tag after PropertyGroup opening tag
    sed -i "s|<PropertyGroup>|<PropertyGroup>\n    <Version>$NEW_VERSION</Version>|" "$project_file"
  fi
  echo "Updated $project_file"
done

# Update Docker image tags in docker-compose files
find . -name "docker-compose*.yml" -o -name "compose*.yaml" | while read -r compose_file; do
  sed -i "s|image: metameta/.*:$CURRENT_VERSION|image: metameta/\\1:$NEW_VERSION|g" "$compose_file"
  echo "Updated $compose_file"
done

echo "Version updated to $NEW_VERSION"
echo "Don't forget to commit these changes!" 