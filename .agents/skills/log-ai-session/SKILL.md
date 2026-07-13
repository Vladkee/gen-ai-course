---
name: log-ai-session
description: >
  Use at the end of every AI work session to append a structured entry to
  homework-N/AI-CONVERSATION.md. Captures model, mode, reasoning effort, what
  was accomplished, and key decisions. Highlights model changes visually.
argument-hint: "Brief description of what was done, e.g. 'implemented ticket import API'"
---

# Log AI Session Skill

Appends a structured session entry to `homework-N/AI-CONVERSATION.md`.

## When to Use

- After every AI-assisted work session (even short ones)
- When switching AI models mid-session (log each segment separately)
- When wrapping up a homework before creating a PR

## Procedure

### Step 1 — Gather Session Info
Collect (ask user if unknown):

- **Date**: Today's date
- **Homework**: Which homework was worked on
- **AI Tool**: GitHub Copilot / Claude Desktop / Claude CLI / other
- **Model**: e.g. `Claude Sonnet 4.5 (copilot)`, `Claude Opus 4.5`, `GPT-4.1`
- **Mode**: Agent / Edit / Chat / CLI
- **Reasoning Effort**: High / Medium / Low / Default (if set explicitly)
- **What was accomplished**: 2-5 bullet points
- **Key decisions**: Architecture choices, library selections, approach decisions
- **Challenges**: Any problems encountered

### Step 2 — Determine Target File

Log to the **per-homework** file based on which homework is being worked on:

| Homework | Log file |
|----------|----------|
| HW1 | `homework-1/AI-CONVERSATION.md` |
| HW2 | `homework-2/AI-CONVERSATION.md` |
| HW3 | `homework-3/AI-CONVERSATION.md` |
| HW4 | `homework-4/AI-CONVERSATION.md` |
| HW5 | `homework-5/AI-CONVERSATION.md` |
| HW6 | `homework-6/AI-CONVERSATION.md` |

If the per-homework `AI-CONVERSATION.md` does not exist yet, create it using this header:

```markdown
# AI-CONVERSATION.md — Homework N: [Title]

Session-by-session log of all AI-assisted work for this homework.  
See [../../AI-SUMMARY.md](../../AI-SUMMARY.md) for the cross-homework summary.

---

```

### Step 3 — Detect Model Change
Read the last entry in the target file. If the model differs from the previous entry, add a prominent model-change callout before the new entry:

```markdown
---
> ⚠️ **MODEL CHANGE**: `[Previous Model]` → `[New Model]`
---
```

### Step 4 — Append Entry
Use the template from [assets/ai-conversation-template.md](./assets/ai-conversation-template.md).
