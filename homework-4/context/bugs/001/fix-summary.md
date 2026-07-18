# Fix Summary — Batch 001

**Agent:** Bug Fixer (pipeline stage 4) · **Model:** claude-sonnet-5 · **Date:** 2026-07-18
**Input:** [implementation-plan.md](implementation-plan.md) (read in full before applying)
**Test command:** `npm test` — run after every change, as required.

---

## Changes Made

### Change 1 — BUG-001-A: inclusive `to` bound

| | |
|---|---|
| **File / location** | `src/store.js:27` (`listExpenses`) |
| **Before** | `if (to && e.date >= to) return false;` |
| **After** | `if (to && e.date > to) return false;` |
| **Test result** | ✅ 6/6 pass |

### Change 2 — BUG-001-B: integer-cents accumulation in `summarize`

| | |
|---|---|
| **File / location** | `src/store.js:35-45` (`summarize` body) |
| **Before** | raw double accumulation: `total += e.amount;` / `byCategory[e.category] = (byCategory[e.category] \|\| 0) + e.amount;` |
| **After** | per-expense `Math.round(e.amount * 100)`, integer-cent sums (`totalCents`, `centsByCategory`), single `/ 100` conversion per result field; response shape unchanged |
| **Test result** | ✅ 6/6 pass |

### Change 3 — SEC-001: auth rewrite

| | |
|---|---|
| **File / location** | `src/auth.js` (entire file) |
| **Before** | hardcoded `ADMIN_TOKEN` constant; `console.log` of presented token; loose `token == ADMIN_TOKEN` |
| **After** | token from `process.env.ADMIN_TOKEN`, fail closed when unset or token non-string; no logging; `crypto.timingSafeEqual` over equal-length buffers with length pre-check |
| **Test result** | ✅ 6/6 pass |

All three "before" snippets matched the live source exactly — no plan/source mismatches.
No files were changed other than the two listed. `src/server.js` intentionally untouched
(plan confirms response shapes and the caller contract are preserved).

## Overall Status

✅ **All 3 plan items applied; baseline suite green after each change and at the end (6 pass / 0 fail).**

## Manual Verification

```powershell
# BUG-001-A — month-end expense included
npm start   # terminal 1
curl -X POST http://localhost:3000/expenses -H "Content-Type: application/json" -d '{"amount":12,"category":"food","date":"2026-07-31"}'
curl "http://localhost:3000/expenses?from=2026-07-01&to=2026-07-31"   # → contains the expense

# BUG-001-B — exact decimal total
curl -X POST http://localhost:3000/expenses -H "Content-Type: application/json" -d '{"amount":10.10,"category":"food","date":"2026-07-02"}'
curl -X POST http://localhost:3000/expenses -H "Content-Type: application/json" -d '{"amount":20.20,"category":"travel","date":"2026-07-03"}'
curl "http://localhost:3000/summary"   # → "total":42.3 (12 + 10.10 + 20.20), no float artifacts

# SEC-001 — fail closed without env token, no token in logs
curl -X DELETE http://localhost:3000/expenses/1 -H "X-Admin-Token: guess123"   # → 404; stdout silent
# restart with $env:ADMIN_TOKEN="local-dev-secret"; correct header → 204
```

## References

- `context/bugs/001/implementation-plan.md`
- `context/bugs/001/bug-context.md`
- Changed files: `src/store.js`, `src/auth.js`
- Pre-fix snapshots: `context/bugs/001/before/`
