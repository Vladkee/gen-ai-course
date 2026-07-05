# Sample Data Generator for Ticketing API
# This script creates sample tickets via the API

$baseUrl = "http://localhost:5002/api"

Write-Host "Creating sample tickets..." -ForegroundColor Green

# Sample tickets
$tickets = @(
    @{
        Subject = "Cannot access dashboard"
        Description = "Getting 403 Forbidden error when trying to view dashboard"
        CustomerName = "Alice Cooper"
        CustomerEmail = "alice.cooper@example.com"
    },
    @{
        Subject = "Overcharged on last invoice"
        Description = "Invoice shows `$199 but should be `$99"
        CustomerName = "Bob Marley"
        CustomerEmail = "bob.marley@company.com"
    },
    @{
        Subject = "How to change username"
        Description = "Want to update my username but can't find the option"
        CustomerName = "Carol King"
        CustomerEmail = "carol.king@test.com"
    },
    @{
        Subject = "Feature request for export"
        Description = "Would like to export data in Excel format"
        CustomerName = "David Bowie"
        CustomerEmail = "david.bowie@example.com"
    },
    @{
        Subject = "API endpoint returning 500"
        Description = "The /v1/users endpoint is consistently returning HTTP 500"
        CustomerName = "Eva Green"
        CustomerEmail = "eva.green@dev.com"
    }
)

foreach ($ticket in $tickets) {
    $body = $ticket | ConvertTo-Json
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/tickets" -Method Post -Body $body -ContentType "application/json"
        Write-Host "Created ticket: $($ticket.subject)" -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to create ticket: $($ticket.subject)" -ForegroundColor Red
        Write-Host "  Error: $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Sample data creation complete!" -ForegroundColor Green
Write-Host "Created $($tickets.Count) tickets" -ForegroundColor Cyan

