$base = "http://localhost:5000"

function Invoke-Api($method, $path, $body = $null) {
    $params = @{ Method = $method; Uri = "$base$path"; ContentType = "application/json" }
    if ($body) { $params.Body = ($body | ConvertTo-Json) }
    try {
        $response = Invoke-RestMethod @params
        Write-Host "$method $path -> OK" -ForegroundColor Green
        $response | ConvertTo-Json -Depth 5
    } catch {
        Write-Host "$method $path -> ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

Write-Host "=== Banking API Sample Data ===" -ForegroundColor Cyan
Write-Host ""

Invoke-Api POST /transactions @{ toAccount = "ACC-10001"; amount = 1500.00; currency = "USD"; type = "Deposit" }
Invoke-Api POST /transactions @{ toAccount = "ACC-10002"; amount = 800.00; currency = "EUR"; type = "Deposit" }
Invoke-Api POST /transactions @{ fromAccount = "ACC-10001"; amount = 200.50; currency = "USD"; type = "Withdrawal" }
Invoke-Api POST /transactions @{ fromAccount = "ACC-10001"; toAccount = "ACC-10002"; amount = 100.00; currency = "USD"; type = "Transfer" }

Invoke-Api GET /transactions
Invoke-Api GET /accounts/ACC-10001/balance
Invoke-Api GET /accounts/ACC-10001/summary

Write-Host "--- Validation Error Demo ---" -ForegroundColor Yellow
Invoke-Api POST /transactions @{ toAccount = "BADACCOUNT"; amount = -50; currency = "XYZ"; type = "Deposit" }
