---
description: >
  Use when generating a PR description for a homework submission. Covers
  the required sections, formatting rules, and examples based on the
  approved HW1 PR format, enhanced with screenshots and run instructions.
---

# PR Description Standards

## Title Format

```
Homework N: [Brief Feature Description] ([Tech Stack])
```

Examples:
- `Homework 2: Customer Support Ticketing System (ASP.NET Core 8)`
- `Homework 5: MCP Server Configuration (Python FastMCP + GitHub + Filesystem)`
- `Homework 6: AI-Powered Banking Pipeline (ASP.NET Core 8 + Angular)`

---

## Required Sections

Every PR description **must** include all of the following sections in order:

### 1. Summary
One-paragraph description of what was implemented. Include: what it is, main capabilities, storage/infrastructure approach.

```markdown
## Summary

Implemented a [description] using [tech stack] with [storage approach].
All required endpoints/features are working and validated.
```

### 2. Endpoints / Features Table (for API homeworks)
```markdown
## Endpoints

| Method | Path | Description | Status |
|--------|------|-------------|--------|
| POST | /tickets | Create ticket | ✅ Required |
| POST | /tickets/import | Bulk import CSV/JSON/XML | ✅ Required |
| GET | /tickets | List with filters | ✅ Required |
```

### 3. What Was Built
Bullet list of technical accomplishments. Be specific — mention validation rules, patterns used, data formats supported.

```markdown
## What Was Built

- Transaction model with fields: id, fromAccount, toAccount, amount, currency, type, timestamp, status
- Field-level validation: positive amount, max 2 decimal places, ACC-XXXXX format, ISO 4217 currency
- In-memory storage using ConcurrentDictionary for thread-safe access
- Filtering by account ID, type, and date range (combinable)
```

### 4. AI Tools Used (Required)
Table of AI tools, models, and phases. **Must list every AI tool session separately.**

```markdown
## AI Tools Used

| Phase | Tool | Model | Mode |
|-------|------|-------|------|
| Planning & scaffolding | GitHub Copilot | Claude Sonnet 4.5 | Agent |
| Code review | GitHub Copilot | Claude Sonnet 4.5 | Chat |
| Security review | GitHub Copilot | Claude Sonnet 4.5 | Agent |

See [AI-CONVERSATION.md](AI-CONVERSATION.md) for the full session log.
```

### 5. Challenges
Numbered list of concrete problems encountered and how they were resolved.

```markdown
## Challenges

1. **[Challenge name]**: [What happened] — [How it was resolved]
2. **[Challenge name]**: [What happened] — [How it was resolved]
```

### 6. How to Run
Step-by-step instructions. Must include prerequisites, start command, and data seeding.

```markdown
## How to Run

See [HOWTORUN.md](HOWTORUN.md) for full steps.

**Prerequisites:** [list: .NET 8 SDK / Node 20 / Python 3.11 etc.]

1. `cd homework-N`
2. Run `.\demo\start.ps1` — starts the API on http://localhost:5000
3. Run `.\demo\sample-data.ps1` — seeds example data and shows responses
4. Or open `demo/sample-requests.http` in VS Code with REST Client extension

**API Base URL:** `http://localhost:5000`
```

### 7. Screenshots (Required per instructor feedback)
Inline images from `docs/screenshots/`. **At least 2 screenshots required.**

```markdown
## Screenshots

### API Running
![API Started](docs/screenshots/api-started.png)

### Sample Requests
![Sample Requests](docs/screenshots/sample-requests.png)

### [Other relevant screenshots]
![Description](docs/screenshots/filename.png)
```

---

## Enhanced PR Checklist (pre-submission)

- [ ] Title follows `Homework N: [Description] ([Tech Stack])` format
- [ ] All 7 sections are present
- [ ] Endpoints table includes all implemented endpoints with ✅/⭐ status
- [ ] AI Tools Used table references the correct model names and modes
- [ ] Challenges section is specific (not generic)
- [ ] How to Run includes exact commands that actually work
- [ ] At least 2 screenshots attached inline
- [ ] `AI-CONVERSATION.md` is up to date and linked in the PR body

---

## Instructor Feedback Incorporated (from HW1 review)

> "Please add screenshots into PR descriptions for future HWs" — @Alexey-Popov

- Screenshots are **mandatory** for all HW2+ PRs
- Inline in the PR description body (not just in the repo folder)
- Cover: API startup, sample requests/responses, any UI if applicable
