# AI-CONVERSATION.md — Homework 1: Banking Transactions API

Session-by-session log of all AI-assisted work for this homework.  
See [../../AI-SUMMARY.md](../../AI-SUMMARY.md) for the cross-homework summary.

---

## Format Reference

Model changes between sessions are marked with:
> ⚠️ **MODEL CHANGE**: `[Previous Model]` → `[New Model]`

---

## Session: 2026-06-27 — Initial Scaffolding

| Setting | Value |
|---------|-------|
| **AI Tool** | Claude CLI (terminal) |
| **Model** | Claude Sonnet 4.6 |
| **Mode** | CLI / Agentic |
| **Reasoning Effort** | Default |
| **Session Duration** | ~2 hours |

### What Was Accomplished

- Scaffolded full ASP.NET Core 8 project from scratch
- Implemented all 4 required endpoints (POST /transactions, GET /transactions, GET /transactions/:id, GET /accounts/:id/balance)
- Added field-level validation (amount rules, ACC-XXXXX format, ISO 4217 currency check)
- Implemented in-memory storage using `ConcurrentDictionary` for thread safety
- Added optional Account Summary endpoint
- Created demo scripts (`start.ps1`, `sample-requests.http`, `sample-data.ps1`)

### Key Decisions

- **ConcurrentDictionary over List**: Chosen for thread-safe in-memory storage without locking overhead
- **ISO 4217 validation**: Full currency list extracted to `Iso4217Currencies.cs` as a static HashSet for O(1) lookup
- **Record types for DTOs**: Immutable request/response models

### Challenges

- **Lost session**: Terminal window was closed; CLI chat session could not be recovered — no screenshots from Phase 1

### Notes

Sonnet 4.6 scaffolded the full project quickly. Validation logic (especially the ISO 4217 list) required multiple follow-up prompts to get right.

---

> ⚠️ **MODEL CHANGE**: `Claude Sonnet 4.6 (CLI)` → `Claude Opus 4.8 (Desktop)`

---

## Session: 2026-06-28 — Review & Polish

| Setting | Value |
|---------|-------|
| **AI Tool** | Claude Desktop |
| **Model** | Claude Opus 4.8 |
| **Mode** | Chat |
| **Reasoning Effort** | Default |
| **Session Duration** | ~1.5 hours |

### What Was Accomplished

- Identified and fixed field-name mismatch: `fromAccountId`/`toAccountId` → `fromAccount`/`toAccount` (alignment with spec)
- Aligned port configuration across `launchSettings.json`, `HOWTORUN.md`, and demo scripts (port 5000/5001)
- Updated `sample-requests.http` and `sample-data.ps1` with corrected field names
- Ran 15-case curl test suite covering happy path, filters, balance calculation, and all validation errors
- Rewrote `README.md` with project structure, transaction model field table, and request/response examples

### Key Decisions

- **Opus for review phase**: Opus 4.8 was noticeably better at cross-referencing spec vs implementation and catching inconsistencies than Sonnet 4.6

### Challenges

- **Port inconsistency**: `launchSettings.json` had port 5063 while docs referenced 5000 — fixed to 5000/5001 across all files
- **Field-name drift**: AI-generated code used slightly different naming than the spec — cross-checking is essential

### Prompts That Worked Well

> "Compare the field names in `Transaction.cs` against the sample JSON in TASKS.md and list any mismatches"

---
