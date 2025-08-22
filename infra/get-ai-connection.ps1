Param(
    [Parameter(Mandatory = $true)]
    [string]$ResourceGroup,

    [Parameter(Mandatory = $true)]
    [string]$AccountName
)

# This helper prints the connection string: [Endpoint=...;Key=...;]
# It handles both "primaryKey" and older "key1" shapes returned by the CLI.

# Get the keys JSON
$keysJson = az cognitiveservices account keys list -g $ResourceGroup -n $AccountName -o json | ConvertFrom-Json

if ($null -ne $keysJson.primaryKey) {
    $primaryKey = $keysJson.primaryKey
}
elseif ($null -ne $keysJson.key1) {
    $primaryKey = $keysJson.key1
}
else {
    Write-Error "No key found in response. Full JSON: $keysJson"
    exit 1
}

# Get the endpoint
$endpoint = az cognitiveservices account show -g $ResourceGroup -n $AccountName --query "properties.endpoint" -o tsv

if ([string]::IsNullOrEmpty($endpoint)) {
    Write-Error "Endpoint not found for account $AccountName in resource group $ResourceGroup"
    exit 1
}

Write-Output "[Endpoint=$endpoint;Key=$primaryKey;]"
