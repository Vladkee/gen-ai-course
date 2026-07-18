# Test Report — Batch 001

**Agent:** Unit Test Generator (pipeline stage 6) · **Model:** claude-sonnet-5 · **Date:** 2026-07-18
**Skill applied:** [`skills/unit-tests-FIRST.md`](../../../skills/unit-tests-FIRST.md)
**Scope:** changed files per [fix-summary.md](fix-summary.md) — `src/store.js`, `src/auth.js` only.

---

## Generated Tests

### `tests/generated/store-fixes.test.js` — targets `src/store.js` (6 tests)

| Test | Covers |
|---|---|
| `listExpenses: expense dated exactly 'to' → included` | **Regression BUG-001-A** |
| `listExpenses: expense dated after 'to' → excluded` | Boundary (adjacent negative) |
| `listExpenses: from === to selects exactly that single day` | Boundary degenerate case |
| `summarize: 10.10 + 20.20 → total exactly 30.3` | **Regression BUG-001-B** (exact reported values) |
| `summarize: per-category subtotals are exact decimals` | Subtotal path of the same fix |
| `summarize: date-filtered summary includes the 'to' day` | Composition of both fixes |

### `tests/generated/auth.test.js` — targets `src/auth.js` (6 tests)

| Test | Covers |
|---|---|
| `correct token with ADMIN_TOKEN set → true` | Happy path of the rewrite |
| `wrong token of same length → false` | Comparison correctness |
| `wrong token of different length → false` | **Regression SEC-001** (length pre-check path) |
| `ADMIN_TOKEN unset → false even for formerly hardcoded value` | Fail-closed + hardcoded secret truly gone |
| `non-string token (missing header) → false` | Caller passes `undefined` when header absent |
| `empty-string token → false` | Adjacent negative |

## Run Results

```
npm test   →  node --test "tests/**/*.test.js"
tests 18 · pass 18 · fail 0 · duration ~93 ms
```

- 12 generated + 6 baseline; baseline tests untouched and green.
- **Pre-fix failure proof:** the regression scenarios were executed against the preserved
  pre-fix snapshot (`context/bugs/001/before/store.js`):
  `to`-bound query *excludes* the July 31 expense (`false`) and the total is
  `30.299999999999997` — i.e., the regression tests fail on pre-fix code and pass post-fix.

## FIRST Compliance

| Letter | How satisfied |
|---|---|
| **Fast** | Pure in-memory function calls; no network/fs/server/sleeps; full suite ≈ 93 ms |
| **Independent** | `beforeEach(reset())` for store state; `ADMIN_TOKEN` set per test and restored in `afterEach`; no cross-test reads |
| **Repeatable** | Fixed literal dates/amounts/tokens; no clock, locale, randomness, or port usage |
| **Self-validating** | Every test ends in `assert/strict` equality with exact expected values (incl. exact `30.3`, not approximate) |
| **Timely** | Written in the same change set as the fixes; scoped strictly to the two changed files; each seeded bug has a regression test proven to fail pre-fix |

Generator checklist from the skill: all five items ✅ (changed-files-only scope, pre-fix-failing
regression tests, naming + `tests/generated/` placement, runs via plain `npm test`, this section).

## Coverage of Symptoms

| Symptom (bug-context) | Regression test |
|---|---|
| BUG-001-A month-end expense missing | `store-fixes.test.js` — "expense dated exactly 'to' → included" |
| BUG-001-B `30.299999999999997` total | `store-fixes.test.js` — "10.10 + 20.20 → total exactly 30.3" |
| SEC-001 token handling | `auth.test.js` — fail-closed, hardcoded-value-rejected, length/content mismatch tests |

## References

- `context/bugs/001/fix-summary.md`, `bug-context.md`, `before/store.js`
- `skills/unit-tests-FIRST.md`
- `tests/generated/store-fixes.test.js`, `tests/generated/auth.test.js`
