# Bug Context — Batch 001

Target application: **SpendLite** (`homework-4/src/`), a zero-dependency Node.js expense-tracker API.
Reported by users / internal security review on 2026-07-18. This file describes **symptoms only** —
root-cause analysis is the Bug Researcher's job.

Pristine copies of the affected source files as they were at report time are preserved in
[`before/`](before/) for before/after comparison.

---

## BUG-001-A — Monthly report misses the last day of the range

**Reported by:** finance user
**Severity (reported):** High

> "I asked for all expenses from 2026-07-01 to 2026-07-31 and the expense I entered on
> July 31st is missing. If I set the end date to 2026-08-01 it shows up. Every month-end
> entry disappears from my reports."

**Reproduce:**
1. `POST /expenses` with `{ "amount": 12, "category": "food", "date": "2026-07-31" }`
2. `GET /expenses?from=2026-07-01&to=2026-07-31`
3. Response is `[]` — expected the July 31 expense to be included.

---

## BUG-001-B — Summary total shows absurd decimals

**Reported by:** finance user
**Severity (reported):** Medium

> "The summary said my total is `30.299999999999997`. My expenses were 10.10 and 20.20.
> That is not a real amount of money."

**Reproduce:**
1. `POST /expenses` with amounts `10.10` and `20.20` (any categories/dates).
2. `GET /summary`
3. `total` is `30.299999999999997` — expected `30.3`. Per-category subtotals show the same artifacts.

---

## SEC-001 — Admin token handling flagged by security review

**Reported by:** internal security review
**Severity (reported):** Critical

> "The admin token that guards `DELETE /expenses/{id}` looks wrong on three counts:
> (1) it appears to be a fixed value shipped in the source code, (2) presented tokens are
> written to the application log in plaintext, and (3) the comparison does not look
> timing-safe. Please remediate before the next release."

**Reproduce / evidence:**
- Grep the repo for the token guard used by the DELETE route.
- Start the server, send `DELETE /expenses/1` with header `X-Admin-Token: guess123`, observe the
  presented token printed to stdout.

---

## Scope note for the pipeline

All three issues are in `src/`. Baseline tests (`tests/store.test.js`) currently pass — none of
them cover the reported scenarios. Fixes must keep the baseline suite green.
