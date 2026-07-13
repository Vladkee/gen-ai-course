# AI-SUMMARY.md — Gen-AI Course Overview

High-level summary across all homework assignments. Updated after each submission.

> Detailed session logs: see `homework-N/AI-CONVERSATION.md` for all homeworks.

---

## Homework Summaries

---

### HW1 — Banking Transactions API

| Field | Value |
|-------|-------|
| **Status** | ✅ Merged |
| **Branch** | `homework-1-submission` |
| **Submitted** | 2026-06-28 |
| **Tech Stack** | ASP.NET Core 8, .NET 8, ConcurrentDictionary (in-memory) |
| **PR** | [#1](https://github.com/Vladkee/gen-ai-course/pull/1) |
| **Session Log** | [homework-1/AI-CONVERSATION.md](homework-1/AI-CONVERSATION.md) |

**What Was Built**  
REST API for banking transactions with 4 required endpoints (POST /transactions, GET /transactions, GET /transactions/:id, GET /accounts/:id/balance) plus optional account summary. Full validation: positive amounts, 2dp max, ACC-XXXXX format, ISO 4217 currency codes.

**AI Tools Used**  
| Phase | Tool | Model |
|-------|------|-------|
| Scaffolding | Claude CLI | Sonnet 4.6 |
| Review & fix | Claude Desktop | Opus 4.8 |

**Skills Created**  
None — agent system didn't exist yet. Created retrospectively during meta session (2026-07-05).

**Key Conclusions**  
- Opus 4.8 was significantly better than Sonnet 4.6 at catching naming inconsistencies and cross-referencing spec vs implementation  
- A stronger model for review phase pays off even when a faster model is fine for scaffolding  
- Always cross-check AI-generated field names against the spec sample JSON immediately

**Blockers / Issues**  
- 🔴 **Lost CLI session**: Terminal closed after Phase 1 — no screenshots from scaffolding phase. Fix: capture screenshots immediately after each phase.  
- 🟡 **ISO 4217 validation**: Needed multiple prompt follow-ups to get the full currency list. Fix: include full constraint details in the first prompt.  
- 🟡 **Field-name drift**: `fromAccountId`/`toAccountId` instead of `fromAccount`/`toAccount`. Fix: provide spec sample JSON in the implementation prompt.

---

### HW2 — Customer Support Ticketing

| Field | Value |
|-------|-------|
| **Status** | ✅ Submitted |
| **Branch** | `homework-2-submission` |
| **Submitted** | 2026-07-05 |
| **Tech Stack** | ASP.NET Core 8, ConcurrentDictionary, xUnit + FluentAssertions, Coverlet |
| **PR** | [#2](https://github.com/Vladkee/gen-ai-course/pull/2) |
| **Session Log** | [homework-2/AI-CONVERSATION.md](homework-2/AI-CONVERSATION.md) |

**What Was Built**  
REST API with 7 endpoints including multi-format bulk import (CSV/JSON/XML with row-level error tracking), keyword-based auto-classification with confidence scoring, and advanced filtering. 73 comprehensive tests achieving 89.1% line coverage (exceeds 85% requirement). 4 documentation files with 3 Mermaid diagrams.

**AI Tools Used**  
| Phase | Tool | Model |
|-------|------|-------|
| Full Implementation & Verification | GitHub Copilot (VS Code) | Claude Sonnet 4.5 (High-Max) |

**Skills Created**  
None — used existing agent system (read-tasks, log-ai-session, generate-pr, learn-from-history).

**Key Conclusions**  
- Controller layer critical for coverage: was 0%, added 17 tests → pushed total from 74.5% to 89.1%
- PowerShell script debugging: API expects PascalCase properties, script had camelCase → 400 errors
- ConcurrentDictionary simpler than EF InMemory for homework scope despite more verbose LINQ filtering
- Applied HW1 lessons: field-name alignment (PascalCase), screenshot discipline (18 screenshots captured)

**Blockers / Issues**  
None significant — all challenges (coverage gap, PowerShell syntax errors) resolved during session.

---

### HW3 — Specification Design

| Field | Value |
|-------|-------|
| **Status** | ✅ Submitted |
| **Branch** | `homework-3-submission` |
| **Submitted** | 2026-07-13 |
| **Tech Stack** | N/A (documents only — Markdown + Mermaid) |
| **PR** | [#3](https://github.com/Vladkee/gen-ai-course/pull/3) |
| **Session Log** | [homework-3/AI-CONVERSATION.md](homework-3/AI-CONVERSATION.md) |

**What Was Built**  
Specification package for a virtual card lifecycle feature (create, freeze/unfreeze, spending limits, transaction visibility) in a regulated FinTech setting: layered `specification.md` (6 mid-level objectives, 16-row edge-case table, assumed-and-justified performance targets, 24 low-level tasks with acceptance criteria, traceability table), `agents.md` domain rules, `.cursor/rules/` editor rules, and README with rationale and best-practices mapping.

**AI Tools Used**  
| Phase | Tool | Model |
|-------|------|-------|
| Agent-system review, spec authoring, docs | Claude Code (CLI) | Claude Fable 5 |

**Skills Created**  
None new — but 4 fixes to the existing system committed first (status-table drift → AI-SUMMARY as single source of truth; docs-only workflow variant; truncated generate-pr Step 7; read-tasks marker clarity).

**Key Conclusions**  
- Cross-model review works: Fable 5 reviewing the Sonnet-authored agent system found structural drift (duplicated status tables) and a truncated skill file
- Compliance placement is a design decision: audit as a mid-level objective, PCI rules as implementation-note guardrails, both re-surfaced as per-task acceptance criteria
- Performance targets read better derived from user intent (freeze = panic action → propagation SLO) than as round numbers

**Blockers / Issues**  
- 🟡 **Branching from main dropped the agent system**: `.agents/` lives only on the unmerged HW2 branch — resolved by stacking `homework-3-submission` on `homework-2-submission`; PR diff shrinks once HW2 merges.

---

### HW4 — 4-Agent Pipeline

| Field | Value |
|-------|-------|
| **Status** | 🔲 Not started |
| **Branch** | — |
| **Tech Stack** | TBD |
| **Session Log** | [homework-4/AI-CONVERSATION.md](homework-4/AI-CONVERSATION.md) |

<!-- To be filled in after completion -->

---

### HW5 — MCP Servers

| Field | Value |
|-------|-------|
| **Status** | 🔲 Not started |
| **Branch** | — |
| **Tech Stack** | Python + FastMCP (custom server), GitHub/Filesystem/Jira MCPs |
| **Session Log** | [homework-5/AI-CONVERSATION.md](homework-5/AI-CONVERSATION.md) |

<!-- To be filled in after completion -->

---

### HW6 — Final Capstone

| Field | Value |
|-------|-------|
| **Status** | 🔲 Not started |
| **Branch** | — |
| **Tech Stack** | TBD (language-agnostic) |
| **Session Log** | [homework-6/AI-CONVERSATION.md](homework-6/AI-CONVERSATION.md) |

<!-- To be filled in after completion -->

---

## Agent System Evolution

Tracks skills and instructions created over time, and which homework motivated them.

| Date | Primitive | Name | Motivated By |
|------|-----------|------|-------------|
| 2026-07-05 | Agent | `homework-engineer` | Meta setup |
| 2026-07-05 | Skill | `read-tasks` | Meta setup |
| 2026-07-05 | Skill | `log-ai-session` | Meta setup |
| 2026-07-05 | Skill | `generate-pr` | Meta setup |
| 2026-07-05 | Skill | `learn-from-history` | Self-improvement pattern |
| 2026-07-05 | Instructions | `homework-workflow` | Meta setup |
| 2026-07-05 | Instructions | `tech-stack` | Meta setup |
| 2026-07-05 | Instructions | `pr-standards` | HW1 instructor feedback |

<!-- New rows added by learn-from-history skill after each retrospective -->

---

## Cross-Homework Patterns

> Updated by `/learn-from-history` after each retrospective.

### What Consistently Works
- Opus-class models for review, Sonnet-class for implementation
- Iterative refinement of validation logic (start broad, then tighten)
- PowerShell demo scripts as the primary verification method

### Recurring Challenges
- **Screenshot discipline**: Must capture immediately — lost sessions = lost evidence
- **Spec alignment**: Always provide spec sample JSON in the initial implementation prompt

### Model Performance Notes
| Task type | Best model | Notes |
|-----------|-----------|-------|
| Scaffolding / initial codegen | Sonnet | Fast, sufficient for structure |
| Review / consistency check | Opus | Better cross-referencing of spec vs code |
| Complex specification writing | Opus / reasoning model | HW3 spec will need this |
| Agent/pipeline orchestration | Sonnet with high reasoning | HW4 |
