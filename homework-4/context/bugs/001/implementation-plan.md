# Implementation Plan — Batch 001

**Agent:** Bug Planner (pipeline stage 3) · **Model:** claude-sonnet-5 · **Date:** 2026-07-18
**Input:** [research/verified-research.md](research/verified-research.md) — Research Quality **B**, planner cleared to proceed (using the verifier's corrected reference `src/server.js:58-60`).

**Test command:** `npm test` (run from `homework-4/`, after **every** change)
**Application order:** Change 1 → Change 2 → Change 3 (independent, but fix functional bugs before the auth rewrite so any auth-related test issue is isolated).

---

## Change 1 — BUG-001-A: make the `to` bound inclusive

**File:** `src/store.js` (line 27)

**Before:**
```js
    if (to && e.date >= to) return false;
```

**After:**
```js
    if (to && e.date > to) return false;
```

**Rationale:** verified-research claim #1/#3 — the `from` bound is inclusive, the `to` bound must
be symmetric; lexicographic compare on `YYYY-MM-DD` makes `>` the correct inclusive-upper test.
`summarize` inherits the fix automatically (claim #4).

---

## Change 2 — BUG-001-B: accumulate money in integer cents

**File:** `src/store.js` (lines 35-41, `summarize` body)

**Before:**
```js
  let total = 0;
  const byCategory = {};
  for (const e of filtered) {
    total += e.amount;
    byCategory[e.category] = (byCategory[e.category] || 0) + e.amount;
  }
  return { count: filtered.length, total, byCategory };
```

**After:**
```js
  let totalCents = 0;
  const centsByCategory = {};
  for (const e of filtered) {
    const cents = Math.round(e.amount * 100);
    totalCents += cents;
    centsByCategory[e.category] = (centsByCategory[e.category] || 0) + cents;
  }
  const byCategory = {};
  for (const [category, cents] of Object.entries(centsByCategory)) {
    byCategory[category] = cents / 100;
  }
  return { count: filtered.length, total: totalCents / 100, byCategory };
```

**Rationale:** verified-research claim #5/#6 — sum in exact integer cents, convert to a decimal
number once per result field. Response shape (`count`, `total`, `byCategory`) is unchanged, so
`src/server.js:62-64` needs no edit.

---

## Change 3 — SEC-001: environment-sourced token, timing-safe check, no logging

**File:** `src/auth.js` (entire file, 8 lines)

**Before:**
```js
// Admin authentication for destructive operations.

const ADMIN_TOKEN = "spendlite-admin-2026";

export function isAdmin(token) {
  console.log("[auth] admin check, token=" + token);
  return token == ADMIN_TOKEN;
}
```

**After:**
```js
// Admin authentication for destructive operations.
// The admin token is provided via the ADMIN_TOKEN environment variable;
// when it is unset, admin access is denied entirely (fail closed).

import { timingSafeEqual } from "node:crypto";

export function isAdmin(token) {
  const expected = process.env.ADMIN_TOKEN;
  if (!expected || typeof token !== "string") return false;
  const presented = Buffer.from(token);
  const secret = Buffer.from(expected);
  if (presented.length !== secret.length) return false;
  return timingSafeEqual(presented, secret);
}
```

**Rationale:** verified-research claims #7-#9 — removes the hardcoded secret (env-sourced, fail
closed when unset), removes the plaintext token log entirely, replaces loose `==` with
`crypto.timingSafeEqual` over equal-length buffers. The length pre-check is required by
`timingSafeEqual` and does not leak secret content, only length. Sole caller
(`src/server.js:68`) passes a header string or `undefined` — both handled.

---

## Post-fix manual verification

1. `POST /expenses` `{ "amount": 12, "category": "food", "date": "2026-07-31" }` then
   `GET /expenses?from=2026-07-01&to=2026-07-31` → the expense **is** included (BUG-001-A).
2. Post amounts `10.10` and `20.20`, `GET /summary` → `total` is exactly `30.3` (BUG-001-B).
3. Start server **without** `ADMIN_TOKEN` set → `DELETE /expenses/1` with any header → 404,
   nothing token-related in stdout. Restart with `ADMIN_TOKEN=<value>` and correct header → 204 (SEC-001).

## Out of scope (noted for the security verifier)

`POST /expenses` accepts `amount` without validation (`src/server.js:50`, `Number(body.amount)`
can yield `NaN`/negatives). Not part of the reported batch; flagging is the security verifier's
call.
