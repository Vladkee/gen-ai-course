# agents.md — AI Agent Guidelines for the Virtual Card Service

How an AI coding agent must behave when implementing [specification.md](specification.md). These rules are binding; when a rule here conflicts with a general best practice, this file wins.

---

## 1. Tech Stack Assumptions

| Layer | Choice | Notes |
|-------|--------|-------|
| Runtime | ASP.NET Core 8 (.NET 8) | Controller → Service → Repository layering |
| Storage | PostgreSQL + EF Core migrations | `audit_events` table is append-only (no UPDATE/DELETE grants) |
| Tests | xUnit + FluentAssertions + Testcontainers | WireMock for the processor stub |
| IDs | ULID (`char(26)`) | Never expose processor identifiers to end users |
| Money | Integer minor units + ISO 4217 code | **Never floats, never `decimal` arithmetic across currencies** |

## 2. Domain Rules (banking — non-negotiable)

1. **Never store, log, return, or echo PAN or CVV.** We hold only: processor card token, last4, expiry month/year. If a task seems to require the PAN, the task is wrong — stop and flag it.
2. **Every state mutation must write its audit event in the same transaction** (outbox pattern via `platform/audit-outbox`). Code that mutates card state without an audit write must not be generated, even as a draft.
3. **Prefer idempotent writes always**: every `POST`/`PUT` handler accepts `Idempotency-Key`; every processor-mutating call sends one. When in doubt, make it idempotent.
4. **The card state machine (spec § 6) is closed.** Do not invent statuses or transitions. Invalid transitions → `409` `invalid_state_transition`.
5. **Ops mutations require a reason code.** No default, no optional.
6. **Freeze blocks new authorizations only** — never generate code or copy implying in-flight authorizations are cancelled.

## 3. Code Style

- Records for DTOs; explicit nullability; no `dynamic`.
- Thin controllers: validation + service call + response mapping only. Business rules live in services; state rules live in the domain layer.
- Errors: RFC 7807 via `platform/problem-json`, stable `code` from the catalogue (spec § 4.5). Never `throw new Exception(...)` in domain/service code.
- Structured JSON logging. Allowed in logs: `card_id`, `last4`, actor, action, correlation IDs. Forbidden: token, PAN, CVV, reveal token, full request bodies of card endpoints.
- All timestamps UTC ISO-8601 `Z`.

## 4. Testing & Verification Expectations

- **Write the tests named in each task's "Done when" before declaring the task complete.** A task without its acceptance tests passing is not done.
- Coverage floor: 85% line coverage on `src/CardService`.
- Every edge case E1–E16 that is automatable gets an integration test named `E{n}_{Description}` so coverage of the edge-case table is greppable.
- Run the full suite after every change set; if tests fail, stop, report, and do not proceed to the next task.
- Never weaken a test to make it pass; if a test seems wrong, flag it with reasoning first.

## 5. How to Treat Edge Cases

- The edge-case table (spec § 7) is the **minimum**, not the ceiling. When you discover an unlisted failure mode, add it to the table in the same PR as the handling code.
- Prefer **explicit declines with stable error codes** over silent success. In FinTech, a wrong "yes" is worse than an annoying "no".
- Empty states return `200` with empty collections, never `404`.
- On any store-vs-processor ambiguity, heal toward **recorded user intent** and emit a `WARN` audit event — never guess silently.

## 6. Security & Compliance Constraints

- Secrets only via the secret manager; a hardcoded key, even in a test, fails review. Test credentials come from Testcontainers/WireMock config.
- Ownership checks on every user-facing card endpoint; cross-user probes return `404` (no existence leak).
- `/internal/*` endpoints must never appear in the public gateway's OpenAPI document.
- Any change touching the reveal-token flow or audit writer requires an explicit human review request in the PR description — do not merge these on green CI alone.

## 7. Workflow

1. Read the full task in spec § 9 including "Done when" **before** writing code.
2. Implement in task order within a group (foundation tasks T1–T5 first — everything depends on them).
3. One task per commit; commit message references the task ID (`T9: user freeze/unfreeze`).
4. After each task: run tests, update `docs/` files the task names, then summarize what changed and which acceptance criteria were verified.
5. If a spec requirement is ambiguous, ask — do not pick an interpretation silently. Record the resolution in the spec via PR.
