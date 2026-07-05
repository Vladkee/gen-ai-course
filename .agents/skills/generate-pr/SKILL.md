---
name: generate-pr
description: >
  Use when finishing a homework assignment to generate a complete, ready-to-paste
  GitHub PR description. Follows the approved format from HW1 with enhancements:
  inline screenshots, model tracking, and structured run instructions.
  References pr-standards.instructions.md for all formatting rules.
argument-hint: "Homework number, e.g. '2' or 'homework-2'"
---

# Generate PR Skill

Generates a complete GitHub Pull Request description for a homework submission.

## When to Use

- When a homework implementation is complete and ready for PR
- When preparing to create a new branch and open a pull request
- When reviewing what the PR description should contain

## Procedure

### Step 1 — Gather Information
Read the following files to understand what was built:
1. `homework-N/TASKS.md` — what was required
2. `homework-N/README.md` — project overview
3. `homework-N/HOWTORUN.md` — run instructions
4. `homework-N/AI-CONVERSATION.md` — AI tools and session history
5. `AI-SUMMARY.md` — cross-homework context

### Step 2 — Identify Screenshots
Check `homework-N/docs/screenshots/` for available screenshots.
If missing, remind the user: "⚠️ You need at least 2 screenshots in `docs/screenshots/` before submitting the PR."

### Step 3 — Generate PR Description
Use the template from [assets/pr-template.md](./assets/pr-template.md) filled with actual values.

### Step 4 — Pre-Submission Checklist
Before delivering the PR description, verify:

- [ ] Title: `Homework N: [Name] ([Tech Stack])`
- [ ] All 7 required sections present
- [ ] Endpoints table has all implemented routes with status indicators
- [ ] AI Tools Used table matches `AI-CONVERSATION.md` entries
- [ ] How to Run commands are copy-paste ready
- [ ] Screenshot paths point to existing files in `docs/screenshots/`
- [ ] `homework-N/AI-CONVERSATION.md` is up to date and linked

### Step 5 — Branch Name Suggestion
Suggest a branch name following: `homework-N-submission`
