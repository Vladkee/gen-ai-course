# Security Report — Batch 001

**Agent:** Security Vulnerabilities Verifier (pipeline stage 5) · **Model:** claude-opus-4-8 · **Date:** 2026-07-18
**Scope source:** [fix-summary.md](fix-summary.md) · **Mode:** report only — no code was modified.

## Scope

Files changed in this batch, each read in full in its current state:

- `src/store.js`
- `src/auth.js`

Context read to judge data flow into changed code: `src/server.js` (caller), `bug-context.md`
(seeded issue SEC-001), `context/bugs/001/before/` (pre-fix state for comparison).

## Findings

### MEDIUM — Unvalidated `amount` reaches monetary aggregation

- **Location:** `src/server.js:50` → `src/store.js:38` (changed code)
- **Description:** `POST /expenses` coerces the client-supplied amount with `Number(body.amount)`
  and stores it without validation. `Math.round(NaN * 100)` is `NaN`, and one `NaN` expense
  poisons `totalCents` permanently — every subsequent `GET /summary` returns `"total": null`
  (JSON-serialized `NaN`). Negative and absurdly large amounts are accepted too. This is a
  data-integrity denial of the summary feature, triggerable by any unauthenticated client.
- **Remediation:** in the POST handler, reject with 400 unless
  `Number.isFinite(amount) && amount > 0` (and, per money hygiene, at most 2 decimal places).
  Out of this batch's plan scope (noted there) — recommend as the next batch.

### INFO — Length pre-check before `timingSafeEqual`

- **Location:** `src/auth.js:13`
- **Description:** the equal-length guard required by `timingSafeEqual` means comparison time
  reveals whether the presented token matches the secret's *length*. Standard practice and low
  value to an attacker; noted for completeness.
- **Remediation:** none required. (An HMAC-both-sides comparison would remove even this signal
  if ever desired.)

## Resolved seeded issue (SEC-001) — verified in source, not from the fix summary

| Reported problem | Pre-fix evidence (`before/auth.js`) | Current state (`src/auth.js`) | Status |
|---|---|---|---|
| Hardcoded secret in source | `const ADMIN_TOKEN = "spendlite-admin-2026";` (line 3) | Token read from `process.env.ADMIN_TOKEN`; fail closed when unset (lines 8-10) | ✅ Resolved |
| Token logged in plaintext | `console.log("[auth] admin check, token=" + ...)` (line 6) | No logging anywhere in the file | ✅ Resolved |
| Timing-unsafe loose comparison | `return token == ADMIN_TOKEN;` (line 7) | `crypto.timingSafeEqual` over equal-length buffers (line 14) | ✅ Resolved |

Grep for the old literal `spendlite-admin-2026` across `src/` and `tests/`: no occurrences remain.

## Checklist coverage (explicit, per category)

| Category | Result |
|---|---|
| Injection (command/SQL/path) | ✅ None — no shell, DB, or filesystem access in changed code |
| Hardcoded secrets | ✅ None remaining (see resolved table) |
| Insecure comparisons | ✅ None — `timingSafeEqual` in auth; date strings and category equality are non-secret |
| Missing input validation | ⚠️ One finding (MEDIUM above) |
| Secrets in logs | ✅ None — the only log statement in changed code was removed |
| Unsafe dependencies | ✅ N/A — zero dependencies; only `node:crypto` added |
| XSS / CSRF | ✅ N/A — JSON API, no HTML rendering, no cookie-based auth |
| Regressions introduced by the fixes | ✅ None found — fixes are strictly narrowing |

## Verdict

**PASS with notes** — the seeded security issue is fully resolved and the fixes introduce no new
vulnerabilities. The MEDIUM input-validation finding predates this batch, is documented in the
implementation plan as out of scope, and should be scheduled as batch 002.
