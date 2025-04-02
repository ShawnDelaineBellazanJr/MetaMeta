# MetaMeta Development Bootstrap Script
# This script helps set up and launch the MetaMeta development environment

# Ensure we're in the correct directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

# Function to check and install prerequisites
function Ensure-Prerequisites {
    Write-Host "🔍 Checking prerequisites..." -ForegroundColor Cyan
    
    # Check Docker
    try {
        $dockerVersion = docker --version
        Write-Host "✅ $dockerVersion" -ForegroundColor Green
    } catch {
        Write-Host "❌ Docker not found or not running. Please install Docker Desktop." -ForegroundColor Red
        Write-Host "   Download from: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
        exit 1
    }
    
    # Check .NET SDK
    try {
        $dotnetVersion = dotnet --version
        Write-Host "✅ .NET SDK $dotnetVersion" -ForegroundColor Green
    } catch {
        Write-Host "❌ .NET SDK not found. Please install the latest .NET SDK." -ForegroundColor Red
        Write-Host "   Download from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
        exit 1
    }
    
    # Check if the current path has any problematic characters
    $currentPath = Get-Location
    if ($currentPath -match "[\(\) ]") {
        Write-Host ""
        Write-Host "⚠️ WARNING: Your current path contains parentheses or spaces:" -ForegroundColor Yellow
        Write-Host "   $currentPath" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "This may cause issues with Docker and DevContainers." -ForegroundColor Yellow
        Write-Host "Recommended: Move this project to a path without parentheses or spaces." -ForegroundColor Yellow
        Write-Host "Example: D:\Projects\MetaMeta" -ForegroundColor Yellow
        Write-Host ""
        
        $continueAnyway = Read-Host "Continue anyway? (y/n)"
        if ($continueAnyway -ne "y") {
            exit 0
        }
    }
}

# Function to start the development container
function Start-DevContainer {
    Write-Host "🚀 Starting development container..." -ForegroundColor Cyan
    
    # Navigate to devcontainer directory
    Push-Location .devcontainer
    
    # Run docker-compose
    try {
        docker-compose -f compose.yaml up --build -d
        if ($LASTEXITCODE -ne 0) {
            throw "Docker Compose failed with exit code $LASTEXITCODE"
        }
        Write-Host "✅ Development container started successfully!" -ForegroundColor Green
    } catch {
        Write-Host "❌ Failed to start development container: $_" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    
    Pop-Location
}

# Function to open the project in Cursor
function Open-InCursor {
    Write-Host "🖥️ Opening project in Cursor..." -ForegroundColor Cyan
    
    try {
        # Check if Cursor is installed
        $cursorPath = "$env:LOCALAPPDATA\Programs\Cursor\Cursor.exe"
        
        if (Test-Path $cursorPath) {
            # Launch Cursor with this folder
            Start-Process $cursorPath -ArgumentList "--folder=."
            Write-Host "✅ Launched Cursor successfully!" -ForegroundColor Green
        } else {
            Write-Host "❓ Cursor not found at expected path. Opening instructions instead." -ForegroundColor Yellow
            Write-Host "   To open in Cursor:"
            Write-Host "   1. Launch Cursor"
            Write-Host "   2. Open folder: $(Get-Location)"
            Write-Host "   3. Click 'Reopen in Container' when prompted"
        }
    } catch {
        Write-Host "❌ Failed to launch Cursor: $_" -ForegroundColor Red
    }
}

# Function to display help info
function Show-ProjectInfo {
    Write-Host ""
    Write-Host "====================== MetaMeta Development ======================" -ForegroundColor Magenta
    Write-Host "🌐 Services:"
    Write-Host "   - REST API:        http://localhost:5000" -ForegroundColor Cyan
    Write-Host "   - gRPC Service:    http://localhost:7100" -ForegroundColor Cyan
    Write-Host "   - Web UI:          http://localhost:3000" -ForegroundColor Cyan
    Write-Host "   - Aspire Dashboard: http://localhost:58000" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "📋 Common Tasks:"
    Write-Host "   - Build:           docker exec metameta_app dotnet build"
    Write-Host "   - Run Tests:       docker exec metameta_app dotnet test"
    Write-Host "   - Start Services:  docker exec metameta_app dotnet run --project src/MetaMeta.AppHost"
    Write-Host ""
    Write-Host "💻 Dev Container:"
    Write-Host "   - Connect:         docker exec -it metameta_app bash"
    Write-Host "   - Stop:            docker-compose -f .devcontainer/compose.yaml down"
    Write-Host ""
    Write-Host "=================================================================" -ForegroundColor Magenta
    Write-Host ""
}

# Main script execution
Clear-Host
Write-Host "🚀 MetaMeta Development Bootstrap" -ForegroundColor Magenta
Write-Host "=================================" -ForegroundColor Magenta
Write-Host ""

# Execute the functions
Ensure-Prerequisites
Start-DevContainer
Open-InCursor
Show-ProjectInfo

Write-Host "✨ Bootstrap complete! Happy coding!" -ForegroundColor Green 