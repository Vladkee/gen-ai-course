# Start the Ticketing API

Write-Host "Starting Ticketing API on port 5002..." -ForegroundColor Green

# Navigate to the API directory
$apiPath = Join-Path $PSScriptRoot "..\src\TicketingApi"
Set-Location $apiPath

# Run the API
dotnet run
