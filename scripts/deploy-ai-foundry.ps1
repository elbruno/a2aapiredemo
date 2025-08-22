<#
Deploy AI Foundry (subscription-scoped) and print a single-line connection string
format: [Endpoint=<endpoint>;Key=<Key>;]

Usage:
  pwsh -File scripts/deploy-ai-foundry.ps1 -Location eastus2 -EnvironmentName brk447demo

This script will:
- Run `az deployment sub create` with `infra/main.bicep`
- Inspect deployment outputs to find the resource group and account name
- Fetch the cognitive services key and endpoint
- Print the connection string: [Endpoint=...;Key=...;]

Notes:
- Requires Azure CLI logged in (`az login`).
- Requires permission to create subscription-scoped deployments and to read keys for the Cognitive Services account.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$Location = 'eastus2',

    [Parameter(Mandatory = $false)]
    [string]$EnvironmentName = 'brk447demo',

    [Parameter(Mandatory = $false)]
    [string]$DeploymentName = 'main'
)

function Test-AzCommandAvailable {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name
    )
    return $null -ne (Get-Command $Name -ErrorAction SilentlyContinue)
}

if (-not (Test-AzCommandAvailable -Name 'az')) {
    Write-Error "Azure CLI 'az' is not available in PATH. Install Azure CLI and login with 'az login'."
    exit 2
}

Write-Host "Starting subscription deployment ($DeploymentName) in location $Location for environment $EnvironmentName..."

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path "$scriptDir\.."
$template = Join-Path $repoRoot 'infra\main.bicep'

if (-not (Test-Path $template)) {
    Write-Error "Template not found: $template"
    exit 3
}

# Create the deployment
$deployCmd = @(
    'deployment', 'sub', 'create',
    '--location', $Location,
    '--template-file', $template,
    '--parameters', "environmentName=$EnvironmentName", 'location=' + $Location,
    '--name', $DeploymentName
)

Write-Host "Running: az $($deployCmd -join ' ')"
$deployResult = az @deployCmd 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) {
    Write-Error "Deployment failed. az exit code: $LASTEXITCODE`n$deployResult"
    exit 4
}

# Query deployment outputs
$show = az deployment sub show --name $DeploymentName --query "properties.outputs" -o json 2>$null | ConvertFrom-Json
if ($null -eq $show) {
    Write-Warning "No outputs found in the subscription deployment. Printing raw deployment show output to help debugging."
    az deployment sub show --name $DeploymentName -o json
    exit 5
}

# Attempt to locate resourceGroup and accountName in outputs (common output names used in this repo)
$rg = $null
$account = $null

# common keys: resourceGroupName, resourceGroup, rgName, aiResourceGroup, cognitiveResourceGroup, accountName, cognitiveName
$possibleRgKeys = @('resourceGroupName', 'resourceGroup', 'rgName', 'aiResourceGroup', 'cognitiveResourceGroup', 'resourceGroupId')
$possibleAccountKeys = @('accountName', 'cognitiveName', 'cognitiveAccountName', 'aifoundryAccountName')

foreach ($k in $possibleRgKeys) {
    if ($show.PSObject.Properties.Name -contains $k) {
        $rg = $show.$k.value
        break
    }
}

# fallback: if there's any output that looks like an RG name (starts with 'rg-')
if (-not $rg) {
    foreach ($p in $show.PSObject.Properties) {
        if ($p.Value -and $p.Value.value -and ($p.Value.value -is [string]) -and ($p.Value.value -like 'rg-*')) {
            $rg = $p.Value.value
            break
        }
    }
}

foreach ($k in $possibleAccountKeys) {
    if ($show.PSObject.Properties.Name -contains $k) {
        $account = $show.$k.value
        break
    }
}

# fallback: search for property values that look like account names (contain 'aifoundry' or 'cog' or 'openai')
if (-not $account) {
    foreach ($p in $show.PSObject.Properties) {
        if ($p.Value -and $p.Value.value -and ($p.Value.value -is [string]) -and ($p.Value.value -match 'aifoundry|cog|openai')) {
            $account = $p.Value.value
            break
        }
    }
}

if (-not $rg -or -not $account) {
    Write-Warning "Could not automatically determine resource group and account name from deployment outputs."
    Write-Host "Deployment outputs:"
    $show | ConvertTo-Json -Depth 10
    Write-Host "Please re-run the script with -ResourceGroup <rg> -AccountName <account> (this script will accept env overrides)."
    exit 6
}

Write-Host "Found resource group: $rg"
Write-Host "Found cognitive account name: $account"

# Get the keys and endpoint
$keysJson = az cognitiveservices account keys list -g $rg -n $account -o json 2>$null | ConvertFrom-Json
if ($null -ne $keysJson -and $null -ne $keysJson.primaryKey) { $primaryKey = $keysJson.primaryKey }
elseif ($null -ne $keysJson -and $null -ne $keysJson.key1) { $primaryKey = $keysJson.key1 }
else {
    Write-Error "No key found for account $account in RG $rg"
    exit 7
}

$endpoint = az cognitiveservices account show -g $rg -n $account --query "properties.endpoint" -o tsv 2>$null
if ([string]::IsNullOrEmpty($endpoint)) {
    Write-Error "Endpoint not found for account $account in resource group $rg"
    exit 8
}

# Print single-line connection string
Write-Output "[Endpoint=$endpoint;Key=$primaryKey;]"

# Exit success
exit 0
