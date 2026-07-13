# GitHub Copilot — Workspace Instructions
# gen-ai-course

You are assisting a senior software engineer (Vlad Radchenko, [@Vladkee](https://github.com/Vladkee))
working through a structured AI-learning course. The goal is to learn best-in-class
AI collaboration patterns while building real software to a professional standard.

---

## Agent System

This workspace uses a custom agent system in `.agents/`. Before any homework work:

1. Consult [`.agents/README.md`](.agents/README.md) for the full system overview
2. Use the `homework-engineer` agent for implementation tasks
3. Available slash commands: `/start-homework`, `/finish-homework`, `/read-tasks`, `/log-ai-session`, `/generate-pr`, `/learn-from-history`

---

## Core Rules

### Always
- Read `homework-N/TASKS.md` **fully** before writing any code
- Follow the Research → Plan → Implement → Verify → Log → PR workflow
- Log every AI session to `homework-N/AI-CONVERSATION.md` (per-homework)
- Apply senior-level code quality: SOLID, clean naming, proper error handling
- Include `HOWTORUN.md`, `demo/` scripts, and `AI-CONVERSATION.md` for every code homework

### Never
- Start coding before presenting and confirming an implementation plan
- Skip the `AI-CONVERSATION.md` update at session end
- Submit a PR without at least 2 screenshots (instructor requirement from HW1 review)
- Modify `TASKS.md` files (they are read-only assignment specs)

---

## Tech Stack Defaults

| Layer | Technology |
|-------|-----------|
| Backend API | ASP.NET Core 8 (.NET 8) |
| Frontend | Angular 17+ |
| Testing | xUnit + FluentAssertions |
| In-memory storage | `ConcurrentDictionary<TKey, TValue>` |
| HTTP demo | VS Code REST Client (`.http` files) |
| Scripts | PowerShell (`.ps1`) |

Override when homework explicitly requires another stack (e.g. Python for HW5 FastMCP).

---

## Homework Status

| HW | Title | Status | Branch |
|----|-------|--------|--------|
| 1 | Banking Transactions API | ✅ Merged | `homework-1-submission` |
| 2 | Customer Support Ticketing | 🔲 Not started | — |
| 3 | Specification Design | 🔲 Not started | — |
| 4 | 4-Agent Pipeline | 🔲 Not started | — |
| 5 | MCP Servers | 🔲 Not started | — |
| 6 | Final Capstone | 🔲 Not started | — |

---

## AI Session Logging

All AI sessions must be recorded in `homework-N/AI-CONVERSATION.md` (per-homework file).
See `AI-SUMMARY.md` (root) for the cross-homework overview.
When the AI model changes between sessions, add a prominent callout:

```
> ⚠️ MODEL CHANGE: `[Previous]` → `[New]`
```

---

## PR Conventions (enhanced from HW1)

Branch: `homework-N-submission`  
Title: `Homework N: [Feature] ([Stack])`  
Body: Summary / Endpoints / What Was Built / AI Tools Used / Challenges / How to Run / Screenshots  
Reviewers: `@Alexey-Popov` + Copilot automated review
