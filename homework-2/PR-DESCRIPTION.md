# Homework 2: Customer Support Ticketing API (ASP.NET Core 8)

## Summary

Implemented a comprehensive customer support ticket management system using **ASP.NET Core 8** with thread-safe in-memory storage (ConcurrentDictionary). Features include multi-format bulk import (CSV/JSON/XML), keyword-based auto-classification with confidence scoring, and advanced filtering capabilities. All required tasks are complete with **89.1% test coverage** (73 tests).

**Student:** Vlad Radchenko ([@Vladkee](https://github.com/Vladkee))  
**Date:** 2026-07-05

---

## Endpoints

| Method | Path | Description | Status |
|--------|------|-------------|--------|
| `POST` | `/api/tickets` | Create a new support ticket (with optional auto-classify) | ✅ Required |
| `POST` | `/api/tickets/import` | Bulk import tickets from CSV/JSON/XML | ✅ Required |
| `GET` | `/api/tickets` | List all tickets with filtering (status, priority, category, date range, email) | ✅ Required |
| `GET` | `/api/tickets/{id}` | Get specific ticket by ID | ✅ Required |
| `PUT` | `/api/tickets/{id}` | Update ticket fields (status, assignee, etc.) | ✅ Required |
| `DELETE` | `/api/tickets/{id}` | Delete a ticket | ✅ Required |
| `POST` | `/api/tickets/{id}/auto-classify` | Auto-classify existing ticket with confidence score | ✅ Required |

---

## What Was Built

- **Multi-format bulk import engine** — Parses CSV (via CsvHelper), JSON (System.Text.Json), and XML (System.Xml.Linq) with row-level error tracking (includes row numbers and field names for failed imports)
- **Auto-classification service** — Keyword-based categorization with 20+ keywords mapping to categories (Technical, Billing, Account, General) and priorities (Critical, High, Medium, Low), returns confidence score (0.0-1.0 based on keyword count × 0.2)
- **Advanced filtering** — Supports multiple filter combinations: status, priority, category, date range (fromDate/toDate), customer email
- **Comprehensive validation** — Email regex validation, string length limits (subject ≤200, description ≤2000, customer name ≤100), enum validation
- **Thread-safe storage** — ConcurrentDictionary with Interlocked.Increment for ID generation, supports concurrent reads/writes
- **73 comprehensive tests** — 89.1% line coverage, 78.7% branch coverage, 98.9% method coverage (exceeds 85% requirement):
  - 11 CRUD operation tests
  - 9 model validation tests
  - 10 classification logic tests
  - 16 import parser tests (CSV/JSON/XML)
  - 18 controller HTTP response tests
  - 5 integration tests (end-to-end workflows)
  - 5 performance tests (1000 tickets in <5s, 500 filtered in <500ms)
- **Multi-level documentation** — 4 markdown files with 3 Mermaid diagrams:
  - HOWTORUN.md — Setup and quick start guide
  - API_REFERENCE.md — Complete endpoint documentation with examples
  - ARCHITECTURE.md — System design with 3 Mermaid diagrams (high-level architecture, component layers, sequence diagrams for ticket creation + bulk import)
  - TESTING_GUIDE.md — Test strategy, coverage report, best practices
- **Demo scripts** — PowerShell scripts (start.ps1, sample-data.ps1), REST Client file (20 sample requests), sample data files (50 CSV, 20 JSON, 30 XML tickets)

---

## AI Tools Used

| Phase | Tool | Model | Mode | Reasoning Effort |
|-------|------|-------|------|------------------|
| Planning & Implementation | GitHub Copilot (VS Code) | Claude Sonnet 4.5 (High-Max) | Edits / Agentic | Default |
| Verification & Testing | GitHub Copilot (VS Code) | Claude Sonnet 4.5 (High-Max) | Edits / Agentic | Default |

**Session Duration:** ~4-5 hours (complete end-to-end implementation)

See [AI-CONVERSATION.md](homework-2/AI-CONVERSATION.md) for the full session log.

---

## Challenges

1. **Coverage below 85% threshold**: Initial test suite achieved 74.5% coverage due to untested controller layer. **Resolution:** Added 17 comprehensive controller tests covering all HTTP endpoints (create, get, list, update, delete, auto-classify, import) to reach 89.1% line coverage.

2. **PowerShell demo script syntax errors**: sample-data.ps1 failed with parser errors due to (a) camelCase property names (API expects PascalCase), (b) unescaped `$` in strings (`$199` interpreted as variable), (c) special Unicode characters (✓/✗) causing encoding issues. **Resolution:** Fixed to PascalCase (Subject, Description, CustomerName, CustomerEmail), escaped dollar signs (`` `$199 ``), removed Unicode characters.

3. **Tech stack decision**: Chose ConcurrentDictionary over EF Core InMemoryDatabase for consistency with HW1 approach, despite requiring more complex LINQ filtering logic. Trade-off: simpler setup, familiar patterns vs. more verbose query code.

---

## How to Run

See [HOWTORUN.md](homework-2/HOWTORUN.md) for full steps.

**Prerequisites:**
- .NET 8 SDK
- PowerShell
- (Optional) VS Code with REST Client extension

**Quick Start:**
```powershell
# 1. Build & test
cd homework-2
dotnet build
dotnet test --collect:"XPlat Code Coverage"

# 2. View coverage report
reportgenerator -reports:"tests\TicketingApi.Tests\TestResults\**\coverage.cobertura.xml" -targetdir:"tests\TicketingApi.Tests\TestResults\CoverageReport" -reporttypes:"Html"
start tests\TicketingApi.Tests\TestResults\CoverageReport\index.html

# 3. Start API
cd demo
.\start.ps1

# 4. Load sample data (new terminal)
cd demo
.\sample-data.ps1

# 5. Try demo requests
# Open demo/sample-requests.http in VS Code → click "Send Request"
```

**API Base URL:** `http://localhost:5002`  
**Swagger UI:** `http://localhost:5002/swagger`

---

## Screenshots

### Coverage Report (89.1%)
![Coverage Report](docs/screenshots/Screenshot%202026-07-05%20181249.png)

### API Running (Swagger UI)
![Swagger UI](docs/screenshots/Screenshot%202026-07-05%20181314.png)

### Sample Ticket Creation
![Create Ticket](docs/screenshots/Screenshot%202026-07-05%20181330.png)

### Auto-Classification in Action
![Auto-Classify](docs/screenshots/Screenshot%202026-07-05%20181550.png)

### Bulk Import (CSV)
![CSV Import](docs/screenshots/Screenshot%202026-07-05%20181605.png)

### Advanced Filtering
![Filtering](docs/screenshots/Screenshot%202026-07-05%20182457.png)

### Test Results (All 73 Passing)
![Test Results](docs/screenshots/Screenshot%202026-07-05%20182516.png)

### Live API Demo
![API Demo](docs/screenshots/Screenshot%202026-07-05%20183203.png)

---

## Pre-Submission Checklist

- ✅ All 7 required endpoints implemented and tested
- ✅ Multi-format import (CSV/JSON/XML) with row-level error tracking
- ✅ Auto-classification with confidence scoring
- ✅ Test coverage >85% (achieved 89.1%)
- ✅ 4 documentation files + 3 Mermaid diagrams
- ✅ HOWTORUN.md with copy-paste ready commands
- ✅ Demo scripts (PowerShell + REST Client)
- ✅ Sample data files (CSV/JSON/XML)
- ✅ 18 screenshots included
- ✅ AI-CONVERSATION.md session log complete

---

**Ready for review!** All requirements met, comprehensive test coverage, complete documentation, and live demo verified.
