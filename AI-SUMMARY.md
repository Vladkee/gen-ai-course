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
| **Status** | 🔲 Not started |
| **Branch** | — |
| **Tech Stack** | TBD (ASP.NET Core 8 preferred) |
| **Session Log** | [homework-2/AI-CONVERSATION.md](homework-2/AI-CONVERSATION.md) |

<!-- To be filled in after completion -->

---

### HW3 — Specification Design

| Field | Value |
|-------|-------|
| **Status** | 🔲 Not started |
| **Branch** | — |
| **Tech Stack** | N/A (documents only) |
| **Session Log** | [homework-3/AI-CONVERSATION.md](homework-3/AI-CONVERSATION.md) |

<!-- To be filled in after completion -->

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
