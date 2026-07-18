# HOWTORUN — Homework 4

## Prerequisites

- **Node.js ≥ 20** (built-in `node:test` runner; developed on v24) — no `npm install` needed,
  the app has zero dependencies
- **Claude Code CLI** installed and authenticated — only needed for `npm run pipeline`

All commands run from `homework-4/`.

## Run the app

```powershell
npm start                          # SpendLite on http://localhost:3000
```

To enable the admin `DELETE` endpoint, set the token first (there is no default — fail closed):

```powershell
$env:ADMIN_TOKEN = "local-dev-secret"; npm start
```

Smoke test:

```powershell
curl -X POST http://localhost:3000/expenses -H "Content-Type: application/json" -d '{"amount":10.10,"category":"food","date":"2026-07-18"}'
curl "http://localhost:3000/expenses?from=2026-07-01&to=2026-07-18"
curl http://localhost:3000/summary
curl -X DELETE http://localhost:3000/expenses/1 -H "X-Admin-Token: local-dev-secret"
```

## Run the tests

```powershell
npm test                           # 18 tests (6 baseline + 12 pipeline-generated), ~100 ms
```

## Run the pipeline (single command)

```powershell
npm run pipeline
```

Runs all six stages in order (Researcher → Research Verifier → Planner → Fixer → Security
Verifier → Test Generator) via `claude -p`, one explicit model per stage, agents loading their
skills automatically. Artifacts land in `context/bugs/001/`, per-stage logs in
`context/bugs/001/logs/`. The runner stops at the first stage that fails or doesn't produce its
expected artifact.

### Re-running from the pristine seeded-bug state

The repo ships **post-pipeline** (bugs fixed, artifacts present). To reproduce the run from
scratch:

```powershell
# 1. restore the seeded-bug sources
Copy-Item context\bugs\001\before\* src\

# 2. remove previous pipeline outputs (keep bug-context.md and before/)
Remove-Item context\bugs\001\research\*, context\bugs\001\implementation-plan.md, `
            context\bugs\001\fix-summary.md, context\bugs\001\security-report.md, `
            context\bugs\001\test-report.md -Force
Remove-Item tests\generated -Recurse -Force

# 3. verify the "before" state: baseline tests green, bugs present
npm test

# 4. run the pipeline
npm run pipeline
```

A different bug batch can be targeted with `BUG_BATCH`, e.g.
`$env:BUG_BATCH = "002"; npm run pipeline` (expects `context/bugs/002/bug-context.md`).
