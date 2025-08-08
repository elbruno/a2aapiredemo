# PowerShell script to fetch NLWeb assets for local Docker build
# This script clones the NLWeb repo, copies code/ and static/ to the local build context, then deletes the temp clone

$ErrorActionPreference = 'Stop'

# Set variables
$nlwebRepo = "https://github.com/nlweb-ai/NLWeb.git"
$tempDir = "$env:TEMP\nlweb_temp_clone"

# Get the script's directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$targetCode = Join-Path $scriptDir "code"
$targetStatic = Join-Path $scriptDir "static"

# Clean up any previous temp clone
if (Test-Path $tempDir) {
    Remove-Item -Recurse -Force $tempDir
}

# Clone NLWeb repo
Write-Host "Cloning NLWeb repo to temp directory..."
git clone --depth 1 $nlwebRepo $tempDir

# Copy code/ and static/ to local build context
Write-Host "Copying code/ and static/ directories..."
Copy-Item -Recurse -Force "$tempDir\code" $targetCode
Copy-Item -Recurse -Force "$tempDir\static" $targetStatic

# Clean up temp clone
Write-Host "Cleaning up temp directory..."
Remove-Item -Recurse -Force $tempDir

Write-Host "NLWeb assets fetched successfully."
