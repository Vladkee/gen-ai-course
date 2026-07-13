# Developer Notes — AI Tool Usage

## Tools Used

| Phase | Tool | Model |
|-------|------|-------|
| Initial application creation | Claude CLI (terminal) | Claude Sonnet 4.6 |
| Code review & polishing | Claude Desktop | Claude Opus 4.8 |

## Process

### Phase 1 — Application Creation (Claude CLI / Sonnet 4.6)

Used Claude CLI in the terminal to scaffold and build the full API from scratch:

- Prompted Claude to generate the ASP.NET Core 8 project structure
- Iteratively refined requests to get correct validation logic (amount rules, `ACC-XXXXX` format, ISO 4217 currency check)
- Asked Claude to implement in-memory storage using `ConcurrentDictionary` for thread safety
- Prompted for transaction filtering by account, type, and date range
- Added the optional **transaction summary** feature via follow-up prompts

> **Note:** The chat session from this phase was lost when the terminal window was closed — no conversation screenshots are available for this phase.

### Phase 2 — Review & Polish (Claude Desktop / Opus 4.8)

Used Claude Desktop to review and improve the existing code:

- Identified and fixed a field-name mismatch (`fromAccountId`/`toAccountId` → `fromAccount`/`toAccount`) to match the assignment specification exactly
- Aligned port configuration (`5000`/`5001`) across `launchSettings.json`, `HOWTORUN.md`, and sample files
- Updated `sample-requests.http` and `sample-data.ps1` to use corrected field names
- Ran a full curl test suite (15+ cases) covering happy path, filters, balance math, and all validation error paths
- Rewrote `README.md` to include project structure, transaction model field table, and request/response examples

## Lessons Learned

- **Save chat sessions early** — Claude CLI sessions are lost when the terminal closes. Export or copy key exchanges before closing.
- **Refine prompts iteratively** — the validation logic (especially the ISO 4217 currency list) required several follow-up prompts to get right.
- **Verify field names against the spec** — AI-generated code can use sensible but slightly different naming than what the assignment requires; always cross-check the generated model against the spec's sample JSON.
- **Use a stronger model for review** — Opus 4.8 caught the field-name inconsistency and doc/code mismatches that Sonnet 4.6 had introduced during rapid scaffolding.
