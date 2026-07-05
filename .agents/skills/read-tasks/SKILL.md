---
name: read-tasks
description: >
  Use when starting any homework to parse TASKS.md into a prioritized,
  actionable checklist. Extracts required vs optional tasks, constraints,
  tech stack hints, and acceptance criteria. Invoke before writing any code.
argument-hint: "Homework number, e.g. 'homework-2' or just '2'"
---

# Read Tasks Skill

Parses a homework's `TASKS.md` into a structured implementation checklist.

## When to Use

- At the very start of any homework session
- When resuming work after a break and need to remember what's left
- When planning the implementation sequence

## Procedure

### Step 1 — Locate and Read
Read the relevant `homework-N/TASKS.md` file in full.

### Step 2 — Extract Structure
Identify and categorize:

| Category | Markers | Action |
|----------|---------|--------|
| Required tasks | ⭐, "Required", bold | Must implement — mark with `[x] REQUIRED` |
| Optional tasks | 🌟, "Choose at least 1", "Optional" | Implement at least minimum — mark `[ ] OPTIONAL` |
| Constraints | Bold requirements, "must", "only" | Extract as constraints list |
| Tech stack hints | "Choose one", "Node.js/Express", etc. | Note alongside stack preferences |
| Acceptance criteria | "Success criteria:", code format specs | Add to each task |

### Step 3 — Output Checklist
Produce a structured checklist in this format:

```markdown
## 📋 Homework N Checklist

**Tech Stack:** [recommended choice with rationale]

### Required Tasks
- [ ] **Task 1** — [brief description]
  - Constraint: [any specific requirement]
  - Done when: [acceptance criteria]
- [ ] **Task 2** — [brief description]
  ...

### Optional Tasks (implement ≥ 1)
- [ ] **Option A** — [description]
- [ ] **Option B** — [description]

### Key Constraints
- [ ] [Important constraint 1]
- [ ] [Important constraint 2]

### Demo Requirements
- [ ] `demo/start.ps1` — one-command server start
- [ ] `demo/sample-requests.http` — all endpoints
- [ ] `demo/sample-data.ps1` — data seeding
- [ ] `HOWTORUN.md`
- [ ] `README.md` with student name
- [ ] `AI-CONVERSATION.md`
- [ ] `docs/screenshots/` (≥2 screenshots)
```

### Step 4 — Confirm Plan
Present the checklist and ask: "Does this match your understanding? Any tasks to add or prioritize differently?"
