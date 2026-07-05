---
description: "Finish a homework assignment — verifies completion, generates a PR description, logs the session, and creates a branch name suggestion."
name: Finish Homework
argument-hint: "Homework number, e.g. '2'"
agent: agent
tools: [read, search, edit, execute, todo]
model: Claude Sonnet 4.5 (copilot)
---

# Finish Homework Session

You are a senior software engineer completing a gen-ai-course homework assignment and preparing it for PR submission.

## Your Goal

Verify the homework is complete, generate a production-quality PR description, and ensure all documentation is in order.

## Instructions

### 1. Completion Check
Read `homework-{{arg}}/TASKS.md` and verify against the implementation:

Run the pre-submission quality gates:
- [ ] All ⭐ required tasks implemented and working
- [ ] `HOWTORUN.md` complete with accurate commands
- [ ] `demo/start.ps1` runs without errors
- [ ] `demo/sample-data.ps1` seeds data successfully
- [ ] `README.md` includes student name and endpoints table
- [ ] `docs/screenshots/` has at least 2 screenshots
- [ ] `homework-{{arg}}/AI-CONVERSATION.md` has an entry for this session

Report any missing items: "⚠️ Before submitting, complete: [list]"

### 2. Log the Session
Apply the `/log-ai-session` skill to append a session entry to `homework-{{arg}}/AI-CONVERSATION.md`.

Collect:
- AI tool and model used in this session
- Mode (Agent/Edit/Chat)
- What was accomplished
- Key decisions made

### 3. Generate PR Description
Apply the `/generate-pr` skill to produce the full PR description using the template.

Output the complete PR description ready to paste into GitHub, wrapped in a code block for easy copying.

### 4. Branch Suggestion
Suggest: `git checkout -b homework-{{arg}}-submission`

Then: `git add homework-{{arg}}/`  
Then: `git commit -m "Complete Homework {{arg}}: [brief description]"`

### 5. Final Reminder
> Remember to:
> - Take at least 2 screenshots before submitting the PR
> - Paste the PR description into the GitHub PR body
> - Request a review from `@Alexey-Popov`
> - Enable Copilot code review on the PR
