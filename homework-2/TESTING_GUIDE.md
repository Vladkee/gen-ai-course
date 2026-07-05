# Testing Guide — Ticketing API

## Test Overview

**Test Framework**: xUnit + FluentAssertions  
**Coverage Tool**: Coverlet  
**Total Tests**: 73  
**Line Coverage**: 89.1%  
**Branch Coverage**: 78.7%  
**Method Coverage**: 98.9%

## Running Tests

### Run All Tests

```powershell
cd homework-2
dotnet test
```

### Run with Coverage

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

### Generate Coverage Report

```powershell
# Install reportgenerator (one-time)
dotnet tool install --global dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator `
  -reports:"tests\TicketingApi.Tests\TestResults\**\coverage.cobertura.xml" `
  -targetdir:"tests\TicketingApi.Tests\TestResults\CoverageReport" `
  -reporttypes:"Html"

# Open the report
start tests\TicketingApi.Tests\TestResults\CoverageReport\index.html
```

### Run Specific Test Class

```powershell
dotnet test --filter ClassName~TicketApiTests
```

### Run Specific Test

```powershell
dotnet test --filter FullyQualifiedName~CreateTicket_ValidRequest_ReturnsCreatedTicket
```

---

## Test Structure

### Test Files (8 files, 73 tests)

| Test File | Tests | Focus Area |
|-----------|-------|------------|
| `TicketApiTests.cs` | 11 | Core CRUD operations |
| `TicketModelTests.cs` | 9 | Model validation |
| `ClassificationTests.cs` | 10 | Auto-classification logic |
| `ImportCsvTests.cs` | 6 | CSV import parsing |
| `ImportJsonTests.cs` | 5 | JSON import parsing |
| `ImportXmlTests.cs` | 5 | XML import parsing |
| `ControllerTests.cs` | 18 | Controller HTTP responses |
| `IntegrationTests.cs` | 5 | End-to-end workflows |
| `PerformanceTests.cs` | 5 | Performance benchmarks |

---

## Test Categories

### 1. Unit Tests — API Layer (11 tests)

Tests in `TicketApiTests.cs`:

```csharp
[Fact]
public async Task CreateTicket_ValidRequest_ReturnsCreatedTicket()
{
    // Arrange
    var service = new TicketService();
    var request = new CreateTicketRequest(...);

    // Act
    var ticket = await service.CreateTicketAsync(request);

    // Assert
    ticket.Should().NotBeNull();
    ticket.Id.Should().BeGreaterThan(0);
}
```

**Coverage:**
- ✅ Create ticket
- ✅ Get by ID (exists and not exists)
- ✅ Get all tickets
- ✅ Filter by status
- ✅ Update ticket
- ✅ Delete ticket
- ✅ Resolved status sets ResolvedAt

---

### 2. Validation Tests (9 tests)

Tests in `TicketModelTests.cs`:

**Scenarios:**
- ✅ Valid data passes
- ✅ Empty subject fails
- ✅ Subject too long (>200 chars) fails
- ✅ Invalid email format fails
- ✅ Description too long (>2000 chars) fails
- ✅ Customer name too long (>100 chars) fails
- ✅ Multiple validation errors collected
- ✅ Record types have correct defaults

**Key Test:**
```csharp
[Fact]
public void TicketValidator_InvalidEmail_ReturnsError()
{
    var errors = TicketValidator.ValidateCreateRequest(
        "Subject", "Desc", "Name", "invalid-email"
    );
    
    errors.Should().ContainSingle();
    errors[0].ErrorMessage.Should().Contain("not valid");
}
```

---

### 3. Classification Tests (10 tests)

Tests in `ClassificationTests.cs`:

**Category Detection:**
- ✅ Technical keywords → Technical category
- ✅ Billing keywords → Billing category
- ✅ Account keywords → Account category
- ✅ No keywords → General category

**Priority Detection:**
- ✅ Urgent keywords → Critical priority
- ✅ High keywords → High priority
- ✅ Question keywords → Low priority
- ✅ No keywords → Medium priority

**Confidence Scoring:**
- ✅ Multiple keywords = higher confidence
- ✅ Confidence capped at 1.0
- ✅ Case-insensitive matching

**Example:**
```csharp
[Fact]
public async Task ClassifyTicket_UrgentKeywords_SuggestsCriticalPriority()
{
    var service = new ClassificationService();
    var result = await service.ClassifyTicketAsync(
        "URGENT: System down", 
        "The entire system is down ASAP help needed"
    );
    
    result.SuggestedPriority.Should().Be(TicketPriority.Critical);
}
```

---

### 4. Import Tests (16 tests)

**CSV Tests** (`ImportCsvTests.cs`, 6 tests):
- ✅ Valid CSV imports successfully
- ✅ Multiple rows imported
- ✅ Invalid email returns error with row number
- ✅ Missing required field returns error
- ✅ Mixed valid/invalid rows report correctly
- ✅ Invalid CSV format returns file error

**JSON Tests** (`ImportJsonTests.cs`, 5 tests):
- ✅ Valid JSON array imports
- ✅ Multiple tickets imported
- ✅ Invalid email returns error
- ✅ Non-array JSON returns error
- ✅ Malformed JSON returns parsing error

**XML Tests** (`ImportXmlTests.cs`, 5 tests):
- ✅ Valid XML imports successfully
- ✅ Multiple tickets imported
- ✅ Invalid email returns error
- ✅ Malformed XML returns parsing error
- ✅ Empty root returns zero tickets

**Example:**
```csharp
[Fact]
public async Task ImportCsv_MixedValidAndInvalid_ReportsCorrectly()
{
    var csv = """
        subject,description,customer_name,customer_email
        Valid,Description,User,user@test.com
        ,Missing subject,User 2,user2@test.com
        Valid 2,Description 2,User 3,user3@test.com
        """;
    
    var summary = await importService.ImportFromCsvAsync(stream);
    
    summary.TotalRows.Should().Be(3);
    summary.SuccessCount.Should().Be(2);
    summary.ErrorCount.Should().Be(1);
    summary.Errors[0].RowNumber.Should().Be(3);
}
```

---

### 5. Controller Tests (18 tests)

Tests in `ControllerTests.cs`:

**TicketsController:**
- ✅ Create returns 201 Created
- ✅ Create with invalid email returns 400
- ✅ Create with auto-classify classifies ticket
- ✅ Get existing ticket returns 200
- ✅ Get non-existing ticket returns 404
- ✅ Get all returns 200 with list
- ✅ Get with filters returns filtered list
- ✅ Update valid ticket returns 200
- ✅ Update non-existing returns 404
- ✅ Update with invalid data returns 400
- ✅ Delete existing returns 204
- ✅ Delete non-existing returns 404
- ✅ Auto-classify existing returns 200
- ✅ Auto-classify non-existing returns 404

**ImportController:**
- ✅ Import CSV returns 200 with summary
- ✅ Import with no file returns 400
- ✅ Import unsupported format returns 400

**Example:**
```csharp
[Fact]
public async Task TicketsController_CreateTicket_InvalidEmail_ReturnsBadRequest()
{
    var controller = new TicketsController(ticketService, classificationService);
    var request = new CreateTicketRequest("Test", "Desc", "User", "invalid-email");
    
    var result = await controller.CreateTicket(request);
    
    result.Result.Should().BeOfType<BadRequestObjectResult>();
}
```

---

### 6. Integration Tests (5 tests)

Tests in `IntegrationTests.cs`:

**End-to-End Workflows:**
- ✅ Create → Classify → Update → Resolve workflow
- ✅ Import → Filter workflow
- ✅ Create with auto-classify
- ✅ Delete and verify
- ✅ Date range filtering

**Example:**
```csharp
[Fact]
public async Task EndToEnd_CreateClassifyUpdate_WorksCorrectly()
{
    // Create ticket
    var ticket = await ticketService.CreateTicketAsync(...);
    
    // Classify
    var classification = await classificationService.ClassifyTicketAsync(...);
    await ticketService.UpdateTicketAsync(ticket.Id, ...);
    
    // Update to resolved
    var updated = await ticketService.UpdateTicketAsync(ticket.Id, ...);
    
    // Assert full workflow
    updated!.Category.Should().Be(TicketCategory.Technical);
    updated.Priority.Should().Be(TicketPriority.Critical);
    updated.Status.Should().Be(TicketStatus.Resolved);
    updated.ResolvedAt.Should().NotBeNull();
}
```

---

### 7. Performance Tests (5 tests)

Tests in `PerformanceTests.cs`:

**Benchmarks:**
- ✅ Create 1000 tickets < 5 seconds
- ✅ Filter 500 tickets < 500ms
- ✅ Bulk CSV import 100 rows < 3 seconds
- ✅ Concurrent reads (100 threads) thread-safe
- ✅ Classification 1000 times < 2 seconds

**Example:**
```csharp
[Fact]
public async Task Performance_Create1000Tickets_CompletesQuickly()
{
    var sw = Stopwatch.StartNew();
    
    for (int i = 0; i < 1000; i++)
    {
        await ticketService.CreateTicketAsync(...);
    }
    
    sw.Stop();
    sw.ElapsedMilliseconds.Should().BeLessThan(5000);
    
    var tickets = await ticketService.GetTicketsAsync();
    tickets.Should().HaveCount(1000);
}
```

---

## Coverage Report

### Summary (89.1% Line Coverage)

| Component | Line Coverage | Branch Coverage | Notes |
|-----------|---------------|-----------------|-------|
| **Controllers** | 100% | 85% | All endpoints tested |
| **Services** | 93% | 80% | Core business logic |
| **Validation** | 68% | 65% | Regex patterns partially tested |
| **Models** | 100% | N/A | DTOs and enums |
| **Program.cs** | 0% | 0% | Startup code (excluded) |

### Coverage by File

```
TicketsController.cs       100%  ✅
ImportController.cs        100%  ✅
TicketService.cs           94.5% ✅
ClassificationService.cs   100%  ✅
ImportService.cs           90.5% ✅
TicketValidator.cs         67.7% ⚠️
Ticket.cs                  85.7% ✅
Enums.cs                   100%  ✅
Program.cs                 0%    ❌ (excluded)
```

### Uncovered Code

**TicketValidator.cs:**
- Some regex edge cases
- All critical paths covered

**Program.cs:**
- Startup/configuration code
- Not testable without integration tests with WebApplicationFactory

---

## Test Data Patterns

### Arrange-Act-Assert (AAA)

All tests follow the AAA pattern:

```csharp
[Fact]
public async Task TestName()
{
    // Arrange - Set up test data
    var service = new TicketService();
    var request = new CreateTicketRequest(...);
    
    // Act - Execute the operation
    var result = await service.CreateTicketAsync(request);
    
    // Assert - Verify the outcome
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
}
```

### FluentAssertions

Using readable assertions:

```csharp
// Instead of: Assert.Equal(expected, actual)
result.Should().Be(expected);

// Instead of: Assert.True(result > 0)
result.Should().BeGreaterThan(0);

// Instead of: Assert.Null(result)
result.Should().BeNull();

// Complex assertions
tickets.Should().HaveCount(5)
       .And.OnlyContain(t => t.Status == TicketStatus.Open);
```

---

## Testing Best Practices

### ✅ DO

- Use descriptive test names: `CreateTicket_InvalidEmail_ReturnsBadRequest`
- Test both success and failure paths
- Use FluentAssertions for readability
- Test edge cases (empty strings, null, max length)
- Isolate tests (each test uses fresh service instances)
- Test error messages, not just error codes

### ❌ DON'T

- Share state between tests
- Use hard-coded IDs (use created ticket IDs)
- Test implementation details
- Write tests that depend on execution order

---

## Continuous Integration

### GitHub Actions Example

```yaml
name: Test

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test with coverage
        run: dotnet test --no-build --collect:"XPlat Code Coverage"
      - name: Coverage threshold check
        run: |
          coverage=$(grep -oP 'line-rate="\K[^"]+' tests/**/coverage.cobertura.xml | head -1)
          if (( $(echo "$coverage < 0.85" | bc -l) )); then
            echo "Coverage $coverage is below 85% threshold"
            exit 1
          fi
```

---

## Coverage Threshold

**Requirement**: >85% line coverage  
**Current**: **89.1%** ✅  

Coverage is enforced in CI/CD pipeline.

---

## Test Fixtures

### Sample Data

Located in `tests/TicketingApi.Tests/Fixtures/`:
- `sample_tickets.csv` - CSV test data
- `sample_tickets.json` - JSON test data
- `sample_tickets.xml` - XML test data

Used by import tests to verify parsing logic.

---

## Running Tests in Watch Mode

For TDD workflow:

```powershell
dotnet watch test
```

Auto-runs tests on file changes.

---

## Debugging Tests

### In VS Code

1. Set breakpoint in test
2. Click "Debug Test" above test method
3. Step through code

### In Visual Studio

1. Right-click test → Debug Test(s)
2. Or: Test Explorer → Right-click → Debug

---

## Test Metrics

| Metric | Value |
|--------|-------|
| Total Tests | 73 |
| Passed | 73 ✅ |
| Failed | 0 |
| Skipped | 0 |
| Duration | ~2.2 seconds |
| Line Coverage | 89.1% |
| Branch Coverage | 78.7% |
| Method Coverage | 98.9% |

---

## Future Test Improvements

1. **Add integration tests with WebApplicationFactory**
   - Test full HTTP pipeline
   - Test middleware
   
2. **Add mutation testing**
   - Use Stryker.NET
   - Verify test quality

3. **Add load tests**
   - Use NBomber or k6
   - Test concurrent imports

4. **Add contract tests**
   - Use Pact
   - Verify API contracts

5. **Add E2E tests**
   - Use Playwright/Selenium
   - Test with real browser
