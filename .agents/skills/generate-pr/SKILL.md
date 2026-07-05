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

### Step 4 — Update AI-SUMMARY.md
Before generating the PR description, update the root `AI-SUMMARY.md` file with high-level info from `homework-N/AI-CONVERSATION.md`.

**Extract only the most important information:**
- Status → ✅ Complete / Submitted
- Tech stack used
- What was built (2-3 sentences max)
- AI tools and models used (table from session log)
- Skills created (if any new skills were added during this homework)
- Key conclusions (1-3 bullet points: what worked well, what was learned)
- Blockers/Issues (only significant problems, not minor fixes)

**Do NOT copy:**
- Full implementation details
- Complete challenge lists
- Step-by-step accomplishments

**Example HW2 Summary Entry:**
```markdown
### HW2 — Customer Support Ticketing

| Field | Value |
|-------|-------|
| **Status** | ✅ Submitted |
| **Branch** | `homework-2-submission` |
| **Submitted** | 2026-07-05 |
| **Tech Stack** | ASP.NET Core 8, ConcurrentDictionary, xUnit + FluentAssertions |
| **PR** | [#N](link-once-created) |
| **Session Log** | [homework-2/AI-CONVERSATION.md](homework-2/AI-CONVERSATION.md) |

**What Was Built**  
REST API with 7 endpoints including multi-format bulk import (CSV/JSON/XML), keyword-based auto-classification with confidence scoring, and advanced filtering. 73 tests, 89.1% coverage.

**AI Tools Used**  
| Phase | Tool | Model |
|-------|------|-------|
| Full Implementation | GitHub Copilot (VS Code) | Claude Sonnet 4.5 (High-Max) |

**Skills Created**  
None — used existing agent system.

**Key Conclusions**  
- Controller layer critical for coverage (was 0%, added 17 tests → 89.1% total)
- PowerShell script debugging: PascalCase vs camelCase property names caused API 400 errors
- ConcurrentDictionary simpler than EF InMemory for homework scope

**Blockers / Issues**  
None significant — all challenges resolved during session.
```

### Step 5 — Pre-Submission Checklist
Before delivering the PR description, verify:

- [ ] Title: `Homework N: [Name] ([Tech Stack])`
- [ ] All 7 required sections present
- [ ] Endpoints table has all implemented routes with status indicators
- [ ] AI Tools Used table matches `AI-CONVERSATION.md` entries
- [ ] How to Run commands are copy-paste ready
- [ ] Screenshot paths point to existing files in `docs/screenshots/`
- [ ] `homework-N/AI-CONVERSATION.md` is up to date and linked
- [ ] `AI-SUMMARY.md` updated with high-level homework summary

### Step 6 — Create PR Description File and Update PR

**Save PR description to file:**
Create `homework-N/PR-DESCRIPTION.md` with the generated content for version control and reference.

**If PR already exists (user created it manually):**
```bash
gh pr edit <PR_NUMBER> --body-file homework-N/PR-DESCRIPTION.md
```

**If PR doesn't exist yet:**
```bash
gh pr create --base main --head homework-N-submission --title "Homework N: [Title] ([Tech Stack])" --body-file homework-N/PR-DESCRIPTION.md
```

**Set default repository first (one-time):**
```bash
gh repo set-default Vladkee/gen-ai-course
```

**GitHub Web Interface (alternative):**
After pushing the branch, create/edit the PR at:
`https://github.com/Vladkee/gen-ai-course/compare/main...homework-N-submission`

Or view/edit existing PR:
`https://github.com/Vladkee/gen-ai-course/pull/<NUMBER>`

### Step 7 — Final Verification
