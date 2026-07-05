# How to Run — Homework 2: Ticketing API

## Prerequisites

- .NET 8 SDK installed
- PowerShell (Windows) or PowerShell Core (cross-platform)
- (Optional) VS Code with REST Client extension

## Quick Start

### 1. Build the Solution

```powershell
cd homework-2
dotnet build
```

### 2. Run Tests with Coverage

```powershell
cd homework-2
dotnet test --collect:"XPlat Code Coverage"
```

To view coverage report:

```powershell
reportgenerator -reports:"tests\TicketingApi.Tests\TestResults\**\coverage.cobertura.xml" -targetdir:"tests\TicketingApi.Tests\TestResults\CoverageReport" -reporttypes:"Html"
start tests\TicketingApi.Tests\TestResults\CoverageReport\index.html
```

### 3. Start the API

```powershell
cd demo
.\start.ps1
```

The API will start on **http://localhost:5002**.

Swagger UI available at: **http://localhost:5002/swagger**

### 4. Load Sample Data

In a **new terminal** (keep the API running):

```powershell
cd demo
.\sample-data.ps1
```

### 5. Try the Demo Requests

#### Option A: Using VS Code REST Client

1. Open `demo/sample-requests.http` in VS Code
2. Click "Send Request" above any request

#### Option B: Using Swagger UI

1. Navigate to http://localhost:5002/swagger
2. Try the endpoints interactively

#### Option C: Using PowerShell

```powershell
# Create a ticket
$body = @{
    subject = "Test ticket"
    description = "Testing the API"
    customerName = "John Doe"
    customerEmail = "john@example.com"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5002/api/tickets" -Method Post -Body $body -ContentType "application/json"

# Get all tickets
Invoke-RestMethod -Uri "http://localhost:5002/api/tickets" -Method Get
```

### 6. Import Sample Files

```powershell
# Import CSV (PowerShell)
$csvPath = "demo\sample_tickets.csv"
Invoke-RestMethod -Uri "http://localhost:5002/api/tickets/import" -Method Post -InFile $csvPath -ContentType "multipart/form-data"

# Import JSON
$jsonPath = "demo\sample_tickets.json"
Invoke-RestMethod -Uri "http://localhost:5002/api/tickets/import" -Method Post -InFile $jsonPath -ContentType "multipart/form-data"

# Import XML
$xmlPath = "demo\sample_tickets.xml"
Invoke-RestMethod -Uri "http://localhost:5002/api/tickets/import" -Method Post -InFile $xmlPath -ContentType "multipart/form-data"
```

Or use requests #13-15 in `sample-requests.http`.

## Project Structure

```
homework-2/
  src/TicketingApi/         # Main API project
    Controllers/            # API endpoints
    Models/                 # Data models
    Services/               # Business logic
    Validation/             # Input validation
  tests/TicketingApi.Tests/ # Test suite (73 tests, 89.1% coverage)
  demo/                     # Demo scripts and sample data
    start.ps1               # Start the API
    sample-data.ps1         # Load sample tickets
    sample-requests.http    # REST Client demo requests
    sample_tickets.csv      # 50 CSV tickets
    sample_tickets.json     # 20 JSON tickets
    sample_tickets.xml      # 30 XML tickets
  docs/                     # Documentation
    API_REFERENCE.md
    ARCHITECTURE.md
    TESTING_GUIDE.md
```

## Available Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/tickets` | Create a new ticket |
| GET | `/api/tickets` | Get all tickets (with filters) |
| GET | `/api/tickets/{id}` | Get ticket by ID |
| PUT | `/api/tickets/{id}` | Update a ticket |
| DELETE | `/api/tickets/{id}` | Delete a ticket |
| POST | `/api/tickets/{id}/auto-classify` | Auto-classify an existing ticket |
| POST | `/api/tickets/import` | Bulk import from CSV/JSON/XML |

See [API_REFERENCE.md](API_REFERENCE.md) for detailed request/response schemas.

## Stopping the API

Press `Ctrl+C` in the terminal running the API.

## Troubleshooting

### Port Already in Use

If port 5002 is taken, edit `src/TicketingApi/Properties/launchSettings.json` and change the port number.

### Tests Failing

Make sure no other instance of the API is running. Tests use the same in-memory storage.

### Import Not Working

Ensure file paths are correct and files exist in the `demo/` folder.

## Next Steps

- Read [ARCHITECTURE.md](ARCHITECTURE.md) for system design
- Read [TESTING_GUIDE.md](TESTING_GUIDE.md) for test strategy
- Read [API_REFERENCE.md](API_REFERENCE.md) for full API documentation
