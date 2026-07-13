# .agents — Gen-AI Course Homework Agent System

A structured AI collaboration system for working through homework assignments as a senior software engineer. Built on the Superpowers framework philosophy with VS Code Copilot-native primitives.

---

## Structure

```
.agents/
├── agents/
│   └── homework-engineer.agent.md     # Main homework agent (entry point)
├── instructions/
│   ├── homework-workflow.instructions.md   # How to approach each homework
│   ├── tech-stack.instructions.md          # Tech stack preferences & decisions
│   └── pr-standards.instructions.md        # PR description standards
├── skills/
│   ├── read-tasks/
│   │   └── SKILL.md                   # Parse TASKS.md into actionable checklist
│   ├── log-ai-session/
│   │   ├── SKILL.md                   # Log AI session to homework-N/AI-CONVERSATION.md
│   │   └── assets/
│   │       └── ai-conversation-template.md
│   ├── generate-pr/
│   │   ├── SKILL.md                   # Generate standardized PR description
│   │   └── assets/
│   │       └── pr-template.md
│   └── learn-from-history/
│       └── SKILL.md                   # Analyze past logs, create skills from mistakes
└── prompts/
    ├── start-homework.prompt.md       # Kick off a new homework session
    └── finish-homework.prompt.md      # Finalize homework + generate PR

.github/
└── copilot-instructions.md            # Workspace-level Copilot agent instructions

AI-SUMMARY.md                          # Cross-homework high-level summary
homework-N/AI-CONVERSATION.md          # Per-homework session logs (all HW1–6)
```

---

## How to Use

### Starting a homework session
Type `/start-homework` in Copilot Chat (Agent mode), or open `prompts/start-homework.prompt.md`.
This automatically runs `/learn-from-history` first to load lessons from past sessions.

### Finishing a homework session
Type `/finish-homework` to generate a PR description and log the session.

### Logging AI session manually
Type `/log-ai-session` to append a session entry to `homework-N/AI-CONVERSATION.md`.
All homeworks (HW1–6) use their own `AI-CONVERSATION.md` inside the homework folder.

### Learning from past sessions
Type `/learn-from-history` to analyze past `AI-CONVERSATION.md` files and create or update
skills/instructions based on documented mistakes.

---

## Agent Primitives

| Type | Purpose | VS Code Path |
|------|---------|-------------|
| `.agent.md` | Custom agent persona + tool restrictions | `.agents/agents/` |
| `.instructions.md` | Always-available guidelines | `.agents/instructions/` |
| `SKILL.md` | On-demand workflow + bundled assets | `.agents/skills/<name>/` |
| `.prompt.md` | Reusable task templates | `.agents/prompts/` |

---

## Tech Stack Preferences

| Homework | Preferred Stack | Notes |
|----------|----------------|-------|
| HW1 | ASP.NET Core 8 ✅ | Done |
| HW2 | ASP.NET Core 8 | TASKS.md suggests Node/Python, .NET is fine |
| HW3 | N/A (spec only) | Documents only |
| HW4 | .NET 8 + any pipeline runner | Agent pipeline |
| HW5 | Python (FastMCP required for Task 4) | Mixed stack |
| HW6 | ASP.NET Core 8 + Angular | Language-agnostic capstone |

---

## AI Collaboration Principles

1. **Research before code** — always read `TASKS.md` fully before writing a single line
2. **Plan before implement** — produce an implementation plan and get confirmation
3. **Log every session** — update `AI-CONVERSATION.md` after each work session
4. **Highlight model switches** — model changes must be visible in the log
5. **Senior-level code** — SOLID principles, proper error handling, meaningful naming
6. **Evidence before completion** — run tests/verify behavior before claiming done
