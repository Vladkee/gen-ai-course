# Virtual Card Lifecycle — Specification

> Ingest the information from this file, implement the Low-Level Tasks, and generate the code that will satisfy the High and Mid-Level Objectives. This specification is written for a regulated FinTech environment: auditability, data protection, and verification expectations are binding requirements, not suggestions.

**Status**: Approved for implementation (hypothetical)
**Domain**: Consumer virtual payment cards
**Stakeholders**: End-users (cardholders), internal Ops/Compliance, Customer Support (read-only), Fraud team (consumer of audit events)

---

## 1. High-Level Objective

Enable a cardholder to manage the full lifecycle of a virtual payment card — **create, freeze/unfreeze, set spending limits, and view transactions** — through a self-service API, with every state change fully auditable by internal Ops/Compliance.

**Scope boundary**: This feature covers lifecycle management of already-provisioned virtual cards within our platform; card issuing rails, payment authorization decisioning, and physical cards are out of scope (the card processor is an external dependency we integrate with, not build).

---

## 2. Mid-Level Objectives

Each objective is observable: it states what changes in the world when it succeeds.

| ID | Objective | Observable outcome |
|----|-----------|-------------------|
| **MLO-1** | **Card creation** — a verified user can create a virtual card and receive usable card credentials via a secure display flow | A new card exists at the processor and in our store; the user can see masked details immediately and reveal full details once, through a short-lived token |
| **MLO-2** | **Freeze / unfreeze** — a user (or Ops) can instantly suspend and restore a card | After freeze, new authorizations on the card are declined within the propagation SLO; after unfreeze, they succeed again; both actions are visible in the card timeline |
| **MLO-3** | **Spending limits** — a user can set, change, and remove per-transaction and monthly limits | Authorizations exceeding an active limit are declined by the processor using limits we pushed; the current effective limits are always readable via API |
| **MLO-4** | **Transaction visibility** — a user can view a paginated, filterable list of the card's transactions | Settled and pending transactions appear in the list within the freshness SLO; filters (date range, status, amount) return correct subsets |
| **MLO-5** | **Auditability** — every lifecycle state change is recorded as an immutable audit event queryable by Ops/Compliance | For any card, Ops can reconstruct the complete ordered history (who, what, when, from where) with no gaps; audit records cannot be edited or deleted |
| **MLO-6** | **Ops/Compliance controls** — internal staff can freeze any card, view any card's state and history, but never see full card credentials | Ops portal actions succeed with a mandatory reason code; PAN is never rendered in any internal view |

---

## 3. Non-Functional & Policy Requirements

All numeric targets below are **assumed targets** — rationale for each is in [README.md § Rationale](README.md#rationale).

### 3.1 Security & Data Protection

| Requirement | Rule |
|-------------|------|
| PAN handling | Full PAN and CVV are **never stored, logged, or returned** by our services. Card credentials live only at the PCI-compliant processor; we store the processor's card token + last4 + expiry month/year |
| Reveal flow | Full card details display uses a **single-use, 60-second** reveal token that proxies to the processor's PCI-scoped iframe/endpoint; each reveal is itself an audit event |
| AuthN/AuthZ | All endpoints require an authenticated session; card endpoints enforce **resource ownership** (user can only touch their own cards); Ops endpoints require the `ops:cards` role and are on a separate internal gateway |
| Data at rest | Card tokens and cardholder linkage encrypted at rest (AES-256, managed keys, annual rotation) |
| Transport | TLS 1.2+ everywhere; internal service calls use mTLS |
| Secrets | Processor API keys via secret manager only — never in code, config files, or logs |

### 3.2 Audit & Logging

- Every state change (create, freeze, unfreeze, limit change, reveal, Ops action) emits an **append-only audit event**: `{event_id (ULID), card_id, actor_id, actor_type (user|ops|system), action, reason_code?, before_state, after_state, source_ip, timestamp (UTC, ISO-8601)}`.
- Audit events are written **transactionally with the state change** (outbox pattern) — a state change without its audit event must be impossible.
- Retention: **7 years** (assumed regulatory horizon for payment records).
- Application logs must contain `card_id` (internal ID) but **never** card token, PAN, CVV, or reveal-token values.

### 3.3 Reliability

- Card state store: durable, single source of truth for lifecycle state; processor is source of truth for authorization behavior.
- All processor-mutating calls are **idempotent** via an `Idempotency-Key` header (ULID), retried with exponential backoff (max 3 attempts) on 5xx/timeouts.
- If the processor accepts a change but our commit fails, a **reconciliation job** (§ 3.5) heals the divergence within 15 minutes.
- Target availability: **99.9%** monthly for the card management API (assumed — matches a consumer FinTech tier below payment-authorization criticality).

### 3.4 Performance (assumed targets)

| Operation | Target | Why it matters |
|-----------|--------|----------------|
| `POST /cards` (create) | P95 ≤ 2 s, P99 ≤ 4 s end-to-end | Includes a synchronous processor call; users tolerate ~2 s for a "creating your card" moment |
| `POST /cards/{id}/freeze` | API response P95 ≤ 500 ms; **decline effect at processor ≤ 5 s** | Freeze is a panic action (lost phone) — propagation SLO is the real product promise |
| `PUT /cards/{id}/limits` | P95 ≤ 800 ms (includes processor push) | Interactive settings screen |
| `GET /cards/{id}/transactions` | P95 ≤ 400 ms for a page | List screens must feel instant |
| Transaction freshness | New processor transaction visible in list ≤ 60 s (webhook path), ≤ 15 min worst case (reconciliation path) | Users check the list right after paying |
| Read-after-write | A user who freezes a card sees `frozen` on the next read (own-writes consistency, ≤ 1 s) | Avoids "did it work?" double-taps |
| Pagination | Default 25, max 100 items/page, cursor-based | Bounds latency and memory |
| Rate limits | 10 req/s per user on reads; 5 lifecycle mutations/min per card | Abuse control without hurting real usage |

### 3.5 Data Consistency

- **Reconciliation job** every 15 minutes: compares card status and limits between our store and the processor; divergences are auto-healed toward our store's intent and reported as `WARN` audit events.
- Transactions ingest primarily via **processor webhooks** (at-least-once; deduplicated by processor transaction ID); nightly full reconciliation catches missed webhooks.

---

## 4. Implementation Notes (guardrails an agent must not violate)

1. **Money**: integer **minor units** + ISO 4217 currency code (`{"amount": 2500, "currency": "EUR"}` = €25.00). Never floats. Never arithmetic across currencies.
2. **IDs**: ULIDs for all entities (`card_id`, `event_id`, `txn_id`). Processor identifiers stored in distinct, clearly named fields (`processor_card_token`) and never exposed to end users.
3. **State machine**: card status ∈ `pending → active ⇄ frozen → terminated` (terminated is absorbing; `pending` only during async creation). Any transition outside this graph is a `409 Conflict` with error code `invalid_state_transition`.
4. **Idempotency**: all `POST`/`PUT` mutations accept `Idempotency-Key`; replays within 24 h return the original result with `Idempotency-Replayed: true`.
5. **Error semantics**: RFC 7807 problem+json; stable machine-readable `code` field (`card_frozen`, `limit_exceeds_maximum`, `reveal_token_expired`, …); validation errors list per-field details; never leak processor error internals to end users.
6. **Freeze semantics**: freeze blocks **new authorizations only** — in-flight authorizations, settlements of prior authorizations, and refunds still process (industry-standard behavior; users must be told this in UX copy).
7. **Limits semantics**: limits are enforced **at the processor** (we push, they enforce); our API validates bounds (per-transaction ≤ monthly; monthly ≤ product ceiling of €10,000 assumed) before pushing. A limit change applies to future authorizations only.
8. **Concurrency**: card row updates use optimistic locking (`version` column); losers of a race get `409 Conflict` and must re-read.
9. **Logging discipline**: structured JSON logs; `card_id` yes; `last4` yes; token/PAN/CVV/reveal-token **never** (enforced by a log-scrubbing test fixture).
10. **Time**: all timestamps UTC ISO-8601 with `Z`; monthly limit windows are calendar months in the **card's billing timezone** (assumed: user profile timezone, fixed at card creation).

---

## 5. Context

### 5.1 Beginning context (exists before this work)

*Hypothetical but specific — an implementing agent should treat these as real.*

- `platform/user-service` — authenticated users with KYC status; provides `GET /internal/users/{id}` (mTLS)
- `platform/api-gateway` — session auth, per-route rate limiting, public vs. internal (ops) gateway split
- Processor sandbox account with REST API: create card, set status, set limits, card-detail iframe, transaction webhooks (`docs/processor-api.md` assumed available)
- Empty service skeleton `services/card-service/` (ASP.NET Core 8 template, CI pipeline, empty PostgreSQL schema `cards`)
- Shared libraries: `platform/audit-outbox` (transactional outbox writer), `platform/problem-json` (RFC 7807 helpers)

### 5.2 Ending context (exists when done)

- `services/card-service/` implementing all endpoints in § 6 with the layering Controller → Service → Repository
- PostgreSQL schema: `cards`, `card_limits`, `card_transactions`, `audit_events` (append-only, no UPDATE/DELETE grants), `idempotency_keys`
- Webhook consumer + 15-min and nightly reconciliation jobs (hosted services)
- OpenAPI document published to the gateway; Ops endpoints registered on the internal gateway only
- Test suite: unit + integration (Testcontainers PostgreSQL + processor stub), ≥ 85% line coverage; compliance checklist (§ 8.3) signed off
- Runbook `docs/runbook.md` covering webhook backlog, reconciliation alerts, and processor outage behavior

---

## 6. API Surface (reference for tasks)

| Method | Path | Actor | MLO |
|--------|------|-------|-----|
| POST | `/cards` | user | MLO-1 |
| GET | `/cards` / `GET /cards/{id}` | user | MLO-1 |
| POST | `/cards/{id}/reveal-token` | user | MLO-1 |
| POST | `/cards/{id}/freeze` / `/unfreeze` | user, ops | MLO-2, MLO-6 |
| PUT | `/cards/{id}/limits` / `GET .../limits` | user | MLO-3 |
| GET | `/cards/{id}/transactions` | user | MLO-4 |
| GET | `/internal/cards/{id}` `/internal/cards/{id}/audit-events` | ops | MLO-5, MLO-6 |
| POST | `/internal/cards/{id}/freeze` | ops (reason code required) | MLO-6 |

Card lifecycle:

```mermaid
stateDiagram-v2
    [*] --> pending: POST /cards
    pending --> active: processor confirms
    pending --> failed: processor rejects
    active --> frozen: freeze (user/ops)
    frozen --> active: unfreeze (user; ops-frozen cards only by ops)
    active --> terminated: terminate
    frozen --> terminated: terminate
    terminated --> [*]
```

---

## 7. Edge Cases & Failure Modes

Expected behavior states the user-visible outcome **and** the audit/compliance implication.

| # | Scenario | Expected behavior | Audit implication |
|---|----------|-------------------|-------------------|
| E1 | Freeze while an authorization is in flight | Authorization completes (industry standard); subsequent ones decline. API responds 200 with `effective_for: "new_authorizations"` | Freeze event recorded; no gap even if a txn lands seconds later |
| E2 | Concurrent freeze + limit change on same card | One wins; loser gets `409 Conflict` (optimistic lock) and re-reads | Both attempts audited (the failed one as `action=…, outcome=conflict`) |
| E3 | Processor accepts freeze, our DB commit fails | User gets 500; reconciliation detects processor=frozen vs store=active within 15 min and heals to user intent (frozen) | `WARN` reconciliation audit event linking both states |
| E4 | Processor timeout on create | Card stays `pending`; client polls; after 3 retries card → `failed`, user told to retry (idempotency key prevents duplicates) | Create-attempt + failure events with processor correlation ID |
| E5 | Duplicate webhook delivery | Deduplicated by processor txn ID; second delivery is a no-op 200 | Single transaction record; duplicate logged at `DEBUG`, not audited |
| E6 | Limit set below current month's spend | Allowed (applies to future authorizations); response includes `warning: "current_month_spend_exceeds_new_limit"` | Limit-change event stores before/after values |
| E7 | Per-transaction limit > monthly limit | `400` `limit_exceeds_monthly`; nothing pushed to processor | No state change → validation failures logged, not audited |
| E8 | User tries to unfreeze an **ops-frozen** card | `403` `frozen_by_ops`, message directs to support | Attempt audited with `actor_type=user, outcome=denied` |
| E9 | Ops freeze without reason code | `400` — reason code is mandatory for all ops mutations | N/A (request rejected before state change) |
| E10 | Reveal token used twice / after 60 s | `410 Gone` `reveal_token_expired`; user requests a new one | Every reveal issuance **and** consumption audited |
| E11 | Terminated card receives any mutation | `409` `invalid_state_transition` — terminated is absorbing | Attempt audited |
| E12 | Empty states: no cards / no transactions | `200` with empty array + pagination envelope (never 404 for empty lists) | N/A |
| E13 | Stale read after freeze (own-writes) | Must not occur: reads after own mutation hit primary (≤ 1 s consistency, § 3.4) | N/A |
| E14 | Rapid freeze/unfreeze cycling (10×/min) | Rate limit: 5 lifecycle mutations/min/card → `429` with `Retry-After` | Pattern surfaced to fraud team via audit event volume |
| E15 | User's KYC gets revoked | Card creation blocked (`403 kyc_required`); existing cards auto-frozen by system actor | System-actor freeze event, reason `kyc_revoked` |
| E16 | Webhook signature invalid | `401`, payload quarantined for security review, alert raised | Security event log (separate stream), not card audit |

---

## 8. Verification

### 8.1 How we know each mid-level objective is met

| MLO | Verification method |
|-----|---------------------|
| MLO-1 | Integration test: create → processor stub confirms → card readable with masked details; reveal-token e2e test against processor sandbox; manual UX review of secure display |
| MLO-2 | Integration test: freeze → processor stub status change asserted; **propagation SLO test**: sandbox authorization attempted 5 s post-freeze must decline; timeline shows both events |
| MLO-3 | Unit tests on validation bounds (E6, E7); integration test: limits pushed to stub verbatim; sandbox authorization over limit declines |
| MLO-4 | Integration tests: webhook ingest → list within freshness SLO; filter correctness against seeded fixture set (below); pagination cursor stability under concurrent inserts |
| MLO-5 | **Audit completeness test**: property-style test performing N random lifecycle actions, then asserting exactly N audit events reconstruct the state; DB-level test that UPDATE/DELETE on `audit_events` is denied |
| MLO-6 | AuthZ matrix test (every endpoint × every role); grep-style CI check + runtime log-scrubber test asserting no PAN/token patterns in logs or internal API responses |

### 8.2 Test categories & fixtures (as documentation)

- **Unit**: state machine transitions (all valid + all invalid pairs), limit validation, money/ULID formatting, idempotency-key semantics.
- **Integration** (Testcontainers PostgreSQL + WireMock processor stub): every endpoint happy path + E1–E16 where automatable.
- **E2E (sandbox)**: create card → set limit → sandbox auth over limit declines → freeze → auth declines → unfreeze → auth succeeds. Run nightly, not per-commit.
- **Fixtures**: `fixtures/transactions-mixed.json` — 250 transactions across 3 cards: pending/settled/refunded, 4 currencies, boundary amounts (0.01, limits exactly at threshold), out-of-order timestamps.
- **Reconciliation check**: seeded-divergence test (store says active, stub says frozen) → job heals + emits `WARN` event.

### 8.3 Manual compliance review checkpoints

1. Compliance officer walkthrough of the audit trail for one full card lifecycle (create → limit → freeze → unfreeze → terminate) — sign-off recorded in the PR.
2. Security review of the reveal-token flow and secret handling before first production deploy.
3. Ops runbook dry-run: simulate processor outage; verify degraded-mode messaging and reconciliation recovery.

---

## 9. Low-Level Tasks

Grouped by mid-level objective. Every task lists: **Prompt** (what you'd ask the agent), **Files**, **Details/constraints**, **Done when** (acceptance criteria).

### Foundation (serves all MLOs)

**T1 — Database schema & migrations**
- Prompt: "Create EF Core migrations for `cards`, `card_limits`, `card_transactions`, `audit_events`, `idempotency_keys` per spec § 4–5, including the append-only grants for `audit_events`."
- Files: `src/CardService/Data/Migrations/*`, `docs/schema.md`
- Details: ULID PKs stored as `char(26)`; `cards.version` int for optimistic locking; money as `bigint` minor units + `char(3)` currency; `audit_events` has no FK cascade deletes.
- Done when: migration applies cleanly to empty schema; `UPDATE/DELETE audit_events` fails for the app role in an integration test.

**T2 — Card state machine**
- Prompt: "Implement `CardStatus` transitions per the § 6 state diagram as a pure, exhaustively-tested domain type."
- Files: `src/CardService/Domain/CardStateMachine.cs`, tests
- Done when: unit tests cover **all** transition pairs (valid → new state; invalid → domain exception mapped to `409 invalid_state_transition`).

**T3 — Idempotency middleware**
- Prompt: "Add `Idempotency-Key` handling per § 4.4 for all mutations."
- Files: `src/CardService/Infrastructure/IdempotencyMiddleware.cs`, tests
- Done when: replay within 24 h returns byte-identical response + `Idempotency-Replayed: true`; same key with different body → `422 idempotency_key_reuse`.

**T4 — Audit outbox integration**
- Prompt: "Wire `platform/audit-outbox` so every state mutation writes its audit event in the same transaction (§ 3.2)."
- Files: `src/CardService/Infrastructure/AuditWriter.cs`, tests
- Done when: MLO-5 property test passes (N actions ⇒ exactly N events); event schema matches § 3.2 field list.

**T5 — Problem+json error model**
- Prompt: "Implement the § 4.5 error catalogue as RFC 7807 responses with stable `code` values."
- Files: `src/CardService/Api/ErrorCatalogue.cs`, `docs/error-codes.md`
- Done when: every error path in E1–E16 returns its documented code; catalogue doc generated from the same source as the code (no drift).

### MLO-1 — Card creation & secure display

**T6 — `POST /cards`**
- Prompt: "Implement card creation: validate KYC via user-service, call processor create-card idempotently, persist as `pending`, confirm to `active` on processor success."
- Files: `Controllers/CardsController.cs`, `Services/CardCreationService.cs`, tests
- Details: E4 retry/backoff rules; E15 KYC gate; store only token/last4/expiry.
- Done when: happy path P95 ≤ 2 s against stub with 300 ms injected latency; E4 and E15 integration tests pass; response never contains PAN.

**T7 — `GET /cards`, `GET /cards/{id}`**
- Prompt: "Implement card read endpoints with ownership enforcement and masked details only."
- Done when: cross-user access returns `404` (not `403` — no existence leak); empty list per E12.

**T8 — Reveal-token flow**
- Prompt: "Implement `POST /cards/{id}/reveal-token` per § 3.1: single-use, 60 s TTL, proxying to the processor's PCI-scoped display."
- Files: `Services/RevealTokenService.cs`, tests
- Done when: E10 (reuse + expiry → `410`) passes; issuance and consumption both produce audit events; token value absent from all logs (scrubber test).

### MLO-2 — Freeze / unfreeze

**T9 — `POST /cards/{id}/freeze` and `/unfreeze` (user)**
- Prompt: "Implement freeze/unfreeze with processor status push, optimistic locking, and § 4.6 semantics."
- Details: E1 (in-flight auths), E8 (ops-frozen guard), E14 (rate limit), record `frozen_by` actor type.
- Done when: E1, E2, E8, E14 tests pass; freeze API P95 ≤ 500 ms against stub.

**T10 — Freeze propagation verification harness**
- Prompt: "Add the nightly sandbox test asserting an authorization attempted 5 s after freeze is declined (§ 3.4 propagation SLO)."
- Files: `tests/E2E/FreezePropagationTests.cs`, CI nightly job
- Done when: test runs green in nightly pipeline and its failure pages the on-call channel.

### MLO-3 — Spending limits

**T11 — `PUT /cards/{id}/limits` + `GET`**
- Prompt: "Implement limit management: validate bounds (§ 4.7), push to processor, persist effective limits with before/after audit."
- Details: E6 (warning on below-current-spend), E7 (400), monthly window per § 4.10.
- Done when: E6/E7 tests pass; audit event contains before/after limit values; P95 ≤ 800 ms.

**T12 — Limit bounds configuration**
- Prompt: "Externalize product ceilings (per-transaction max, monthly max €10,000 assumed) to configuration with startup validation."
- Done when: invalid config fails startup with a clear message; bounds documented in `docs/limits.md`.

### MLO-4 — Transaction visibility

**T13 — Webhook consumer**
- Prompt: "Implement the processor transaction webhook endpoint: signature verification, dedup by processor txn ID, at-least-once tolerance."
- Details: E5 (duplicates), E16 (bad signature → quarantine + alert).
- Done when: E5/E16 tests pass; ingest-to-visible ≤ 60 s in integration test.

**T14 — `GET /cards/{id}/transactions`**
- Prompt: "Implement the paginated, filterable transaction list (date range, status, min/max amount) with cursor pagination per § 3.4."
- Done when: filter correctness verified against `fixtures/transactions-mixed.json`; cursor stable under concurrent inserts; P95 ≤ 400 ms on 10k-row fixture.

**T15 — Nightly transaction reconciliation**
- Prompt: "Implement the nightly job pulling the processor's transaction report and healing missed webhooks (§ 3.5)."
- Done when: seeded-gap test (delete 3 ingested txns, run job) restores them; summary audit event emitted per run.

### MLO-5 / MLO-6 — Audit & Ops

**T16 — `GET /internal/cards/{id}/audit-events`**
- Prompt: "Implement the Ops audit timeline endpoint: ordered, paginated, filterable by action type, `ops:cards` role required."
- Done when: full lifecycle reconstructable in order; endpoint absent from the public gateway's OpenAPI.

**T17 — Ops freeze with reason codes**
- Prompt: "Implement `POST /internal/cards/{id}/freeze` requiring an enumerated reason code (`fraud_suspected`, `kyc_revoked`, `court_order`, `user_request_via_support`)."
- Details: E9 (missing reason → 400); ops-frozen cards unfreezable only by ops (E8 counterpart).
- Done when: E8/E9 tests pass; reason code appears in the audit event and Ops timeline.

**T18 — KYC-revocation system freeze**
- Prompt: "Subscribe to user-service KYC-revoked events and auto-freeze all the user's active cards as `actor_type=system`."
- Done when: E15 integration test passes; each affected card gets its own audit event with reason `kyc_revoked`.

**T19 — Status/limit reconciliation job (15-min)**
- Prompt: "Implement the § 3.5 store-vs-processor reconciliation with heal-toward-intent and `WARN` audit events."
- Done when: E3 seeded-divergence test passes; divergence count exported as a metric with an alert threshold.

**T20 — AuthZ matrix & log-scrubbing tests**
- Prompt: "Add the MLO-6 verification suite: every endpoint × role matrix test, plus the log-scrubber asserting no PAN/token/reveal-token patterns in captured logs."
- Done when: matrix has zero unexpected-allow cells; scrubber test fails the build when a seeded leak is introduced (verified by mutation).

### Cross-cutting closeout

**T21 — OpenAPI + gateway registration** — public endpoints on public gateway, `/internal/*` on ops gateway only. Done when gateway config reviewed and spec lints clean.
**T22 — Load test against performance targets** — k6 (or similar) scripts for § 3.4 table; done when all P95/P99 targets met on staging-sized data (10k cards, 1M transactions) or deviations documented with remediation tasks.
**T23 — Runbook & degraded modes** — processor-outage behavior (reads work, mutations return `503 processor_unavailable` with retry guidance). Done when Ops dry-run (§ 8.3.3) completed.
**T24 — Compliance sign-off package** — assemble § 8.3 evidence (audit walkthrough export, security review notes). Done when compliance officer sign-off is recorded.

---

## 10. Traceability Summary

| MLO | Tasks | Key edge cases | Verification |
|-----|-------|----------------|--------------|
| MLO-1 | T6–T8 (+T1–T5) | E4, E10, E12, E15 | § 8.1 row 1, scrubber (T20) |
| MLO-2 | T9–T10 | E1, E2, E8, E14 | Propagation SLO test (T10) |
| MLO-3 | T11–T12 | E6, E7 | Sandbox over-limit decline |
| MLO-4 | T13–T15 | E5, E12, E16 | Fixture filters, freshness SLO |
| MLO-5 | T4, T16, T19 | E3, E11 | Property test, append-only test |
| MLO-6 | T17, T18, T20 | E8, E9 | AuthZ matrix, log scrubber |
