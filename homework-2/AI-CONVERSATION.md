# AI-CONVERSATION.md — Homework 2: Customer Support Ticketing

Session-by-session log of all AI-assisted work for this homework.  
See [../../AI-SUMMARY.md](../../AI-SUMMARY.md) for the cross-homework summary.

---

## Format Reference

Each entry captures:
- **AI Tool** — GitHub Copilot / Claude Desktop / Claude CLI / other
- **Model** — exact model + version (e.g. `Claude Sonnet 4.5 (copilot)`)
- **Mode** — Agent / Edit / Chat / CLI
- **Reasoning Effort** — High / Medium / Low / Default (when explicitly configured)

Model changes between sessions are marked with:
> ⚠️ **MODEL CHANGE**: `[Previous Model]` → `[New Model]`

---

## Session: 2026-07-05 — Complete Implementation & Verification

| Setting | Value |
|---------|-------|
| **AI Tool** | GitHub Copilot (VS Code) |
| **Model** | Claude Sonnet 4.5 |
| **Mode** | Edits / Agentic |
| **Reasoning Effort** | Default |
| **Session Duration** | ~4-5 hours |
| **Status** | ✅ Complete (ready for screenshots & PR) |

### What Was Accomplished

**Planning Phase:**
- Applied `/learn-from-history` — surfaced 3 key lessons from HW1 (field-name drift, screenshot discipline, full requirements up front)
- Read `homework-2/TASKS.md` and extracted checklist via `/read-tasks`
- Evaluated tech stack: ConcurrentDictionary vs EF InMemory → chose ConcurrentDictionary for consistency with HW1
- Produced 10-step implementation plan with folder structure

**Implementation Phase:**
- Created project structure (src, tests, demo, docs folders)
- Updated README.md with HW2 content
- Built complete ASP.NET Core 8 API with 7 endpoints:
  - POST /api/tickets (create ticket, with optional auto-classify)
  - GET /api/tickets (list with filters: status, priority, category, date range, customer email)
  - GET /api/tickets/{id} (get single ticket)
  - PUT /api/tickets/{id} (update ticket)
  - DELETE /api/tickets/{id} (delete ticket)
  - POST /api/tickets/{id}/auto-classify (auto-classify existing ticket)
  - POST /api/tickets/import (bulk import CSV/JSON/XML)
- Implemented keyword-based classification service with 20+ keywords, confidence scoring
- Implemented multi-format import service (CSV via CsvHelper, JSON via System.Text.Json, XML via System.Xml.Linq)
- Built comprehensive test suite: 73 tests across 8 test files
  - TicketApiTests (11 tests) - CRUD operations
  - TicketModelTests (9 tests) - Validation logic
  - ClassificationTests (10 tests) - Keyword matching
  - ImportCsvTests (6 tests) - CSV parsing
  - ImportJsonTests (5 tests) - JSON parsing
  - ImportXmlTests (5 tests) - XML parsing
  - ControllerTests (18 tests) - HTTP responses
  - IntegrationTests (5 tests) - End-to-end workflows
  - PerformanceTests (5 tests) - Benchmarks
- **Achieved 89.1% line coverage** (exceeds 85% requirement)
- Created 3 sample data files: sample_tickets.csv (50 tickets), sample_tickets.json (20 tickets), sample_tickets.xml (30 tickets)
- Created demo scripts: start.ps1, sample-requests.http (20 requests), sample-data.ps1
- Created 4 documentation files with 3 Mermaid diagrams:
  - HOWTORUN.md - Setup and quick start guide
  - API_REFERENCE.md - Complete endpoint documentation with examples
  - ARCHITECTURE.md - System design with 3 Mermaid diagrams (high-level architecture, component layers, sequence diagrams)
  - TESTING_GUIDE.md - Test strategy, coverage report, best practices

**Verification Phase:**
- All 73 tests passing
- Coverage report generated: 89.1% line coverage, 78.7% branch coverage, 98.9% method coverage
- Final build successful with 1 minor warning (xUnit async recommendation)
- **API tested live**:
  - Started successfully on port 5002 ✅
  - Sample data script fixed (PascalCase properties, escaped $) and working ✅
  - Created 5 sample tickets via PowerShell ✅
  - Tested auto-classification: URGENT + billing keywords → Critical priority + Billing category (confidence 0.8) ✅
  - Bulk import (CSV/JSON/XML) ready to test via REST Client extension
- Created TESTING-VERIFICATION.md documenting all verified features

### Key Decisions

- **ConcurrentDictionary over EF InMemory**: Simpler, familiar from HW1, sufficient for in-memory requirements despite more complex filtering logic
- **56-test suite expanded to 73**: Added 17 controller tests to push coverage from 74.5% to 89.1%
- **Keyword-based classification**: Simple, transparent, sufficient for homework scope (vs ML/AI models)
- **Three format parsers with row-level error reporting**: Detailed error tracking with row numbers and field names for bulk imports

### Tech Stack

- ASP.NET Core 8 (.NET 8)
- ConcurrentDictionary for in-memory storage
- CsvHelper, System.Text.Json, System.Xml.Linq for import
- xUnit + FluentAssertions + Coverlet (>85% coverage target)

### Notes

Following stricter workflow from HW1 lessons: providing full spec sample data in initial prompts, capturing screenshots immediately after each demo run.

**Implementation Highlights:**
- Full SOLID principles applied: interfaces for all services, single responsibility per class
- Clean separation: Controllers → Services → Storage
- Comprehensive error handling with structured responses
- Thread-safe in-memory storage with `ConcurrentDictionary` + `Interlocked.Increment`
- Row-level import error tracking with field names and row numbers
- Auto-classification with transparent keyword matching (no black-box AI)
- Performance tests verify 1000 tickets created in <5s, 500 tickets filtered in <500ms

**Challenges Overcome:**
- Coverage was initially 74.5% (below 85% requirement) due to untested controllers
- Solution: Added 17 controller tests covering all HTTP endpoints
- Final coverage: 89.1% line coverage ✅
- **sample-data.ps1 PowerShell syntax errors**:
  - Issue: Used camelCase properties (API expects PascalCase)
  - Issue: Unescaped `$` in strings (`$199` interpreted as variable)
  - Issue: Special Unicode characters (✓/✗) causing encoding issues
  - Solution: Fixed to PascalCase (Subject, Description, CustomerName, CustomerEmail), escaped dollar signs (`` `$199 ``), removed Unicode chars

**Next Steps:**
- Test bulk import (CSV/JSON/XML) via REST Client extension (demo/sample-requests.http requests #13-15)
- Capture screenshots (see docs/screenshots/SCREENSHOT-TODO.md):
  - Coverage report (89.1%) → `test_coverage.png`
  - Swagger UI → `swagger_ui.png`
  - Import demo result → `import_demo.png`
  - At least 2 screenshots required per HW1 review
- Generate PR description via `/generate-pr`
- Submit to `homework-2-submission` branch

### Session Summary

**Complete implementation of Homework 2: Customer Support Ticketing API**

✅ **All Requirements Met:**
- 7 REST endpoints (CRUD + auto-classify + bulk import) ✅
- Multi-format import (CSV/JSON/XML with row-level error tracking) ✅
- Auto-classification with confidence scoring ✅
- 73 tests, 89.1% coverage (exceeds 85% requirement) ✅
- 4 documentation files + 3 Mermaid diagrams ✅
- Live API verification successful ✅

**Deliverables Ready:**
- ✅ Working API on port 5002
- ✅ Comprehensive test suite (all passing)
- ✅ Complete documentation (HOWTORUN, API_REFERENCE, ARCHITECTURE, TESTING_GUIDE)
- ✅ Demo scripts and sample data (CSV/JSON/XML)
- ⏳ Screenshots pending (coverage report, Swagger UI, import demo)
- ⏳ PR description pending (via `/generate-pr`)

**Quality Metrics:**
- Line Coverage: **89.1%** (target: >85%)
- Branch Coverage: **78.7%**
- Method Coverage: **98.9%**
- Test Count: **73** (all passing)
- Build: **Success** (1 minor xUnit warning)

---

<!-- Sessions are appended below by the /log-ai-session skill -->
<!-- ============================================================ -->

