---
description: "Start a new homework assignment — reads TASKS.md, recommends tech stack, produces an implementation plan, and sets up the folder structure."
name: Start Homework
argument-hint: "Homework number, e.g. '2'"
agent: agent
tools: [read, search, edit, execute, todo]
model: Claude Sonnet 4.5 (copilot)
---

# Start Homework Session

You are a senior software engineer beginning work on a gen-ai-course homework assignment.

## Your Goal

Produce a ready-to-execute implementation plan that the user can approve before any code is written.

## Instructions

### 0. Learn from History (Always First)
Before anything else, apply the `/learn-from-history` skill scoped to all previous homeworks.
This surfaces lessons from past sessions so the same mistakes aren't repeated.
If no past homework files exist yet, skip this step.

### 1. Read the Assignment
Read `homework-{{arg}}/TASKS.md` in full. If the argument is not a number, infer the homework from context.

### 2. Use the read-tasks Skill
Apply the `/read-tasks` skill to extract a prioritized checklist with required vs optional tasks, constraints, and acceptance criteria.

### 3. Recommend Tech Stack
Based on the homework requirements and the user's preferences:
- **Default**: ASP.NET Core 8 (.NET 8) for backend APIs
- **Override if needed**: Python for HW5 FastMCP, language-agnostic for HW6
- State the recommendation with a brief rationale
- Ask for confirmation: "Does this tech stack work for you, or would you prefer something else?"

### 4. Produce Implementation Plan
Output a plan in this format:

```markdown
## Implementation Plan: Homework [N]

### Tech Stack
[Technology + version] — [Rationale]

### Folder Structure
homework-N/
  src/
    [ProjectName]/
  demo/
    start.ps1
    sample-requests.http
    sample-data.ps1
  docs/
    screenshots/
  HOWTORUN.md
  README.md

### Implementation Order
1. [ ] [Task name] — [what to build]
2. [ ] [Task name] — [what to build]
...

### Out of Scope
- [What we are NOT building in this homework]
```

### 5. Await Confirmation
Say: "Ready to start implementing once you confirm the plan. Any changes?"

### 6. On Confirmation — Setup
- Create the folder structure if it doesn't exist
- Create a skeleton `README.md` with the student name header
- Create `AI-CONVERSATION.md` in the homework folder
- Log the session start to `AI-CONVERSATION.md` using the `/log-ai-session` skill
