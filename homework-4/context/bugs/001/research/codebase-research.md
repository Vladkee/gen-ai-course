# Codebase Research — Batch 001

**Agent:** Bug Researcher (pipeline stage 1) · **Model:** claude-opus-4-8 · **Date:** 2026-07-18
**Input:** [bug-context.md](../bug-context.md) · **Codebase:** `homework-4/src/`, `homework-4/tests/`

---

## BUG-001-A — Month-end expenses missing from range queries

**Root cause:** the upper date bound in `listExpenses` is **exclusive** — it rejects any expense
whose date equals `to`.

**Evidence** — `src/store.js:27`:

```js
if (to && e.date >= to) return false;
```

Dates are `YYYY-MM-DD` strings compared lexicographically (see comment at `src/store.js:2`), so
for `to = "2026-07-31"` an expense dated `2026-07-31` satisfies `e.date >= to` and is filtered
out. The `from` bound at `src/store.js:26` (`e.date < from`) is inclusive, so the two bounds are
asymmetric — exactly matching the report ("setting the end date to 2026-08-01 makes it show up").

**Affected surface:** `GET /expenses?from&to` (`src/server.js:60-62`) and, transitively,
`GET /summary?from&to` — `summarize` delegates filtering to `listExpenses` at `src/store.js:34`.

**Fix direction:** make the upper bound inclusive: `if (to && e.date > to) return false;`

---

## BUG-001-B — Summary total shows `30.299999999999997`

**Root cause:** monetary amounts are accumulated as IEEE-754 doubles.

**Evidence** — `src/store.js:38-39`:

```js
total += e.amount;
byCategory[e.category] = (byCategory[e.category] || 0) + e.amount;
```

`10.10` and `20.20` have no exact binary representation; `10.1 + 20.2 === 30.299999999999997`.
Both the grand total and the per-category subtotals accumulate raw doubles, matching the report
that both show artifacts.

**Affected surface:** `GET /summary` (`src/server.js:62-64`). Amounts enter as raw numbers via
`Number(body.amount)` at `src/server.js:50` with no rounding on ingestion either.

**Fix direction:** accumulate in integer minor units (cents): round each amount to cents with
`Math.round(e.amount * 100)`, sum as integers, divide by 100 once at the end — for both `total`
and `byCategory`.

---

## SEC-001 — Admin token handling

**Root cause:** all three reported problems are confirmed in `src/auth.js` (8 lines, whole file
relevant):

1. **Hardcoded secret** — `src/auth.js:3`:
   ```js
   const ADMIN_TOKEN = "spendlite-admin-2026";
   ```
2. **Token logged in plaintext** — `src/auth.js:6`:
   ```js
   console.log("[auth] admin check, token=" + token);
   ```
   Every authentication attempt writes the presented token to stdout.
3. **Timing-unsafe, loose comparison** — `src/auth.js:7`:
   ```js
   return token == ADMIN_TOKEN;
   ```
   Loose `==` plus short-circuiting string comparison (leaks match-length timing).

**Affected surface:** `DELETE /expenses/{id}` — the only caller is `src/server.js:68`
(`isAdmin(req.headers["x-admin-token"])`). On auth failure the route correctly returns 404
(no existence leak), so the blast radius is confined to `auth.js` itself.

**Fix direction:** read the expected token from `process.env.ADMIN_TOKEN` (fail closed when
unset), compare with `crypto.timingSafeEqual` over equal-length buffers, and remove the log
statement entirely.

---

## Test-coverage note

`tests/store.test.js` passes with all three defects present: the date test at line 25-31 only
exercises the `from` bound, the summary test at line 33-42 uses whole-number amounts, and
nothing tests `auth.js`. This explains why CI stayed green.

## References

- `src/store.js` (lines 2, 26, 27, 34, 38, 39)
- `src/auth.js` (lines 3, 6, 7)
- `src/server.js` (lines 50, 60-68)
- `tests/store.test.js`
- `context/bugs/001/bug-context.md`
