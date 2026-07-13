---
description: >
  Use when working on any gen-ai-course homework assignment. Defines the
  research-plan-implement-log workflow, folder conventions, and quality gates
  every homework submission must meet.
applyTo: "homework-*/**"
---

# Homework Workflow

## Phase 1 — Research (Never Skip)

Before writing any code or documents:

1. Open and read `homework-N/TASKS.md` **in full**
2. Identify all task tiers:
   - ⭐ Required — must implement
   - ⭐⭐ / ⭐⭐⭐ Required with higher complexity
   - 🌟 Optional — implement at least one
3. List any constraints: tech stack, file formats, single-command execution, coverage thresholds
4. Check if a `specification-TEMPLATE-example.md` or similar reference exists in the homework folder

## Phase 2 — Plan

Produce an explicit implementation plan before touching code:

```markdown
## Implementation Plan: Homework N

### Tech Stack Choice
[Technology] — Rationale: [why]

### Folder Structure
homework-N/
  src/
    [ProjectName]/
  demo/
    start.ps1
    sample-requests.http
    sample-data.ps1
  docs/
    screenshots/            # Required for PR submission
  HOWTORUN.md
  README.md

### Tasks (in order)
1. [ ] Task 1 — [brief description]
2. [ ] Task 2 — [brief description]
...
```

Get **explicit confirmation** before proceeding to implementation.

## Phase 3 — Implement

- Build required tasks first, optional tasks last
- Follow `homework-1/src/BankingApi/` structure as the .NET reference
- **Every API project** must have:
  - `demo/start.ps1` — one-command server start
  - `demo/sample-requests.http` — REST Client file with all endpoints
  - `demo/sample-data.ps1` — data seeding script
  - `HOWTORUN.md` — step-by-step setup and run instructions
- Apply .NET 8 coding standards:
  - `record` types for immutable DTOs
  - `ConcurrentDictionary` for in-memory thread-safe storage
  - FluentValidation or data annotations for input validation
  - Proper HTTP status codes (200, 201, 400, 404, 409)
  - Controller → Service → Repository separation for anything beyond trivial

## Phase 4 — Verify

Before claiming any task is done:
- Run the application locally (`demo/start.ps1`)
- Execute `demo/sample-data.ps1` and verify all responses match expectations
- Confirm all required endpoints/features work
- Run the test suite and confirm coverage thresholds are met (HW2: ≥85%)
- **Capture at least 2 screenshots now** — screenshot immediately after each successful demo run; do not defer
- For agent-based homeworks: run the single-command pipeline and check all output files

## Phase 5 — Document & Log

1. Append to `homework-N/AI-CONVERSATION.md` using the `/log-ai-session` skill
2. Update `AI-SUMMARY.md` (root) with a row for this homework if first session
3. Generate PR description using `/generate-pr` skill

## Folder Conventions

All homework folders follow this structure:
```
homework-N/
  TASKS.md                  # Assignment spec (read-only — never modify)
  AI-CONVERSATION.md        # Per-homework session log
  README.md                 # Your project README (create/update)
  HOWTORUN.md               # Step-by-step run guide
  demo/
    start.ps1
    sample-requests.http
    sample-data.ps1
  docs/
    screenshots/            # Required for PR submission
  src/
    [ProjectName]/          # Source code
  specification.md          # HW3/HW6 only
```

## Quality Gates (All Must Pass Before PR)

- [ ] All ⭐ required tasks implemented
- [ ] `HOWTORUN.md` is complete and accurate  
- [ ] `demo/start.ps1` runs without errors
- [ ] `README.md` includes student name, tech stack, and endpoint table (for APIs)
- [ ] `homework-N/AI-CONVERSATION.md` has a new entry for this session
- [ ] `AI-SUMMARY.md` (root) has a row for this homework
- [ ] At least 2 screenshots captured in `docs/screenshots/`

## Docs-Only Homework Variant (HW3 and similar)

When a homework produces **only documents** (no runnable code), Phases 3–4 and the
folder conventions adapt as follows:

- **No `demo/`, `src/`, or `HOWTORUN.md`** — deliverables are the Markdown files named in `TASKS.md`
- **Phase 3 (Implement)** = write the documents; layering and traceability replace code structure
- **Phase 4 (Verify)** = self-review against `TASKS.md`: every required section present, every
  low-level task traces to a mid-level objective, edge cases / verification / performance
  targets are integrated (not a single vague bullet), all internal links resolve
- **Screenshots** (still required — instructor rule): capture the **rendered** documents —
  e.g. the spec preview, Mermaid diagrams, document structure/outline view
- **Quality gates**: replace the `start.ps1` / endpoint-table gates with:
  - [ ] All deliverable files from `TASKS.md` exist and are complete
  - [ ] Every low-level task references the mid-level objective it serves
  - [ ] Edge cases, verification, and performance sections are substantive
