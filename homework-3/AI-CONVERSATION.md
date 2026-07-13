# AI-CONVERSATION.md — Homework 3: Specification-Driven Design

Session-by-session log of all AI-assisted work for this homework.  
See [../AI-SUMMARY.md](../AI-SUMMARY.md) for the cross-homework summary.

---

## Session: 2026-07-13 — Homework 3: Virtual Card Lifecycle Specification

| Setting | Value |
|---------|-------|
| **AI Tool** | Claude Code (CLI) |
| **Model** | Claude Fable 5 |
| **Mode** | CLI (Agent) |
| **Reasoning Effort** | Default |
| **Session Duration** | ~1.5 h |

> ⚠️ **MODEL CHANGE** (vs. HW2): `Claude Sonnet 4.5 (copilot)` → `Claude Fable 5 (Claude Code CLI)`

### What Was Accomplished

- Reviewed the entire `.agents/` system built in the HW2/meta sessions; identified and fixed 4 issues (commit 1): status-table drift across three files (made `AI-SUMMARY.md` the single source of truth), missing docs-only workflow variant (HW3 has no runnable code), truncated Step 7 in the `generate-pr` skill, confusing `[x] REQUIRED` marker in `read-tasks`
- Wrote `specification.md`: 6 mid-level objectives, 16-row edge-case/failure-mode table, assumed-and-justified performance targets, 24 low-level tasks with acceptance criteria, and a traceability summary table
- Wrote `agents.md` (domain rules: PAN handling, transactional audit, idempotency-by-default) and `.cursor/rules/fintech-virtual-cards.md` (naming, FinTech-sensitive defaults, anti-patterns)
- Wrote `README.md` with rationale for layering choices, performance-target derivation, and a 12-row industry-best-practices mapping with file/section references

### Key Decisions

- **Domain: virtual card lifecycle** — richest edge-case surface of the suggested options (freeze vs. in-flight authorizations, ops-vs-user permission boundaries, store-vs-processor divergence)
- **Auditability and ops controls promoted to mid-level objectives** (MLO-5/6) rather than implementation notes — compliance has stakeholders, so it is a feature
- **Compliance concerns split across layers deliberately** (objectives / implementation notes / per-task acceptance criteria) and the split defended in README — this placement question is graded explicitly per TASKS.md
- **Performance targets derived from user intent** (freeze = panic action → propagation SLO is the real promise) and labeled as assumed, per TASKS.md requirement
- **Branch based on `homework-2-submission`, not `main`** — the `.agents/` system only exists on the unmerged HW2 branch; PR diff will shrink to HW3-only once HW2 merges

### Challenges

- **Branching from `origin/main` silently dropped the agent system**: `.agents/`, `.github/`, and `AI-SUMMARY.md` were never merged to main, so the first branch attempt lost them from the working tree — resolved by resetting the branch onto `origin/homework-2-submission`
- **Code-centric workflow didn't fit a docs-only homework**: quality gates demanded `start.ps1` and endpoint tables — resolved by adding a docs-only variant to `homework-workflow.instructions.md` before starting, so the gap is fixed for HW6-style document phases too

### Prompts That Worked Well

> "Please check all the agents, skills, instructions inside the folder. Before that I worked with Sonnet model and created a set of rules. Please check them and let me know what you think."

(Asking the new model to review the previous model's rule system before using it surfaced real drift — the status tables disagreed with reality — and a truncated skill file.)

### Notes

- Fable 5 review of the Sonnet-authored agent system found issues that were structural (duplication → drift) rather than cosmetic; cross-model review seems worth repeating at HW4+
- The `learn-from-history` philosophy paid off in reverse this session: the fixes were committed *first*, separately, so the agent-system improvements are reviewable independently of the HW3 content

---
