#!/bin/bash
# ./scripts/build-clean.sh - Enhanced with conflict detection and security features

RESTORE=true
BUILD=true
DETECT_CONFLICTS=true
VERIFY_INTEGRITY=false  # Requires additional setup

# Parse arguments
while [[ $# -gt 0 ]]; do
  case "$1" in
    --no-restore)
      RESTORE=false
      shift
      ;;
    --no-build)
      BUILD=false
      shift
      ;;
    --no-conflict-detection)
      DETECT_CONFLICTS=false
      shift
      ;;
    --verify-integrity)
      VERIFY_INTEGRITY=true
      shift
      ;;
    *)
      echo "Unknown option: $1"
      exit 1
      ;;
  esac
done

echo -e "\n🧽 INITIATING: MetaMeta Build Cleanse Protocol (v1.1.0)"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SRC_PATH="$SCRIPT_DIR/../src"
TESTS_PATH="$SCRIPT_DIR/../tests"

clean_output() {
  local path="$1"
  echo "🔍 Cleaning build artifacts in $path"
  find "$path" -type d -name "bin" -o -name "obj" | xargs rm -rf 2>/dev/null
  echo "✅ Cleaned: $path"
}

detect_package_conflicts() {
  if [ "$DETECT_CONFLICTS" = true ]; then
    echo -e "\n🔍 Scanning for package conflicts..."
    # Find all .csproj files
    local PROJECT_FILES=$(find $SRC_PATH $TESTS_PATH -name "*.csproj")
    
    # Check if Directory.Packages.props exists
    if [ -f "$SCRIPT_DIR/../Directory.Packages.props" ]; then
      echo "✅ Using central package management"
      
      # Check for any project not using central package management
      local NON_CPM_PROJECTS=0
      for PROJECT_FILE in $PROJECT_FILES; do
        if ! grep -q "<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>" "$PROJECT_FILE"; then
          echo "⚠️ Project not using central package management: $PROJECT_FILE"
          NON_CPM_PROJECTS=$((NON_CPM_PROJECTS+1))
        fi
      done
      
      if [ $NON_CPM_PROJECTS -gt 0 ]; then
        echo "⚠️ Found $NON_CPM_PROJECTS projects not using central package management"
        echo "   Consider running: ./scripts/update-to-central-versioning.sh"
      else
        echo "✅ All projects are using central package management"
      fi
    else
      echo "⚠️ Directory.Packages.props not found - central package management not configured"
    fi
  fi
}

check_package_integrity() {
  if [ "$VERIFY_INTEGRITY" = true ]; then
    echo -e "\n🔒 Verifying package integrity..."
    # This would validate downloaded packages against expected hashes
    # Requires additional setup with a hash verification mechanism
    echo "⚠️ Package integrity verification not implemented in development mode"
  fi
}

echo -e "\n🔍 Locating all build outputs in /src and /tests..."
clean_output "$SRC_PATH"
clean_output "$TESTS_PATH"

# Check for problematic files
for PROJECT in $(find "$SRC_PATH" "$TESTS_PATH" -name "*.csproj" -type f); do
  PROJECT_DIR=$(dirname "$PROJECT")
  ASSETS_PATH="$PROJECT_DIR/obj/project.assets.json"
  NUGET_PROPS="$PROJECT_DIR/obj/project.nuget.cache"
  
  for FILE in "$ASSETS_PATH" "$NUGET_PROPS"; do
    if [ -f "$FILE" ]; then
      rm -f "$FILE"
      echo "🗑️ Deleted lock file: $FILE"
    fi
  done
done

# Detect package conflicts
detect_package_conflicts

# Verify package integrity
check_package_integrity

if [ "$RESTORE" = true ]; then
  echo -e "\n🔄 Executing: dotnet restore with enhanced settings"
  # Use the retry pattern for resilience
  MAX_RETRIES=3
  RETRY_COUNT=0
  RESTORE_SUCCESS=false
  
  while [ $RETRY_COUNT -lt $MAX_RETRIES ] && [ "$RESTORE_SUCCESS" = false ]; do
    RETRY_COUNT=$((RETRY_COUNT+1))
    
    if [ $RETRY_COUNT -gt 1 ]; then
      echo "🔁 Retry attempt $RETRY_COUNT of $MAX_RETRIES"
      # Wait a bit before retrying
      sleep 2
    fi
    
    # Use additional restore flags for better diagnostics
    dotnet restore --force --verbosity minimal
    
    if [ $? -eq 0 ]; then
      RESTORE_SUCCESS=true
      echo "✅ Restore completed successfully"
    else
      echo "⚠️ Restore failed, retrying..."
    fi
  done
  
  if [ "$RESTORE_SUCCESS" = false ]; then
    echo "❌ Restore failed after $MAX_RETRIES attempts"
    exit 1
  fi
fi

if [ "$BUILD" = true ]; then
  echo -e "\n🏗️ Executing: dotnet build with enhanced settings"
  # Use additional build flags
  dotnet build "$SCRIPT_DIR/../MetaMeta.sln" --no-restore --verbosity minimal
  
  if [ $? -eq 0 ]; then
    echo "✅ Build completed successfully"
  else
    echo "❌ Build failed"
    exit 1
  fi
fi

echo -e "\n✅ MetaMeta cleansing ritual complete." 