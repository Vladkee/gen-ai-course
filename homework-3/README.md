# Homework 3 — Specification-Driven Design

**Student**: Vlad Radchenko ([@Vladkee](https://github.com/Vladkee))

**Task summary**: A complete specification package for a **virtual card lifecycle** feature (create, freeze/unfreeze, spending limits, transaction visibility) in a regulated FinTech environment. Documents only — no code. Deliverables: [specification.md](specification.md) (layered spec), [agents.md](agents.md) (AI agent guidelines), [CLAUDE.md](CLAUDE.md) (editor/AI rules for Claude Code), and this README.

---

## Rationale

### Why the spec is layered this way

The spec follows the course template's shape (high-level → mid-level → notes → context → tasks) but extends it where the template was too thin for a regulated domain:

- **Mid-level objectives are phrased as observable outcomes** (spec § 2) — "after freeze, new authorizations decline within the propagation SLO" — because an objective an agent can't observe is an objective it can't verify. MLO-5 (auditability) and MLO-6 (ops controls) are promoted to first-class objectives rather than implementation notes: in a regulated environment, compliance *is* a product feature with its own stakeholders, not a constraint on other features.
- **Where each compliance concern lives was a deliberate split**: auditability is an *objective* (MLO-5, it has users — Ops/Compliance), PCI/data-handling rules are *implementation notes* (§ 3.1, § 4 — invariant guardrails, not features), and both surface again as *per-task acceptance criteria* (e.g. T8 "token value absent from all logs"). This keeps guardrails from being verifiable only "somewhere at the end."
- **24 low-level tasks in 6 groups** (§ 9), each with a prompt, files, constraints, and a checkable "Done when." Foundation tasks (T1–T5) are separated because state machine, idempotency, audit outbox, and error catalogue are dependencies of every feature task — decomposition follows the dependency graph, not the feature list.
- **Traceability is explicit** (§ 10): every task maps to an objective, every objective to edge cases and a verification method. This is the property TASKS.md grades ("traceable from goals down to tasks"), so it gets its own summary table.

### How performance targets were chosen

All numbers are labeled **assumed targets** (spec § 3.4) and were derived from user intent rather than picked round:

- **Freeze**: the API latency (P95 ≤ 500 ms) matters less than the **≤ 5 s decline-propagation SLO**, because freeze is a panic action (lost phone) and the real promise is "the card stops working now." That's why T10 tests propagation, not just the endpoint.
- **Create ≤ 2 s P95**: includes a synchronous processor round trip; consumer FinTech UX research generally tolerates ~2 s behind a progress state, and going async-only would complicate the UX for marginal gain.
- **Reads ≤ 400 ms, cursor pagination 25/100**: list screens must feel instant; caps bound worst-case latency and make the P95 achievable on realistic data (the T22 load test pins this to 10k cards / 1M transactions).
- **Freshness ≤ 60 s webhook / ≤ 15 min reconciliation**: split into a fast path and a guaranteed path because webhooks are at-least-once but not guaranteed — the SLO a user feels and the SLO compliance needs are different numbers.
- **Availability 99.9%**: card *management* is a tier below payment *authorization* (which the processor owns); claiming 99.99% would add cost without a user-visible benefit.

### How verification depth was chosen

Verification is scaled to blast radius: money-adjacent invariants get the strongest checks (property-style audit-completeness test, DB-level append-only test, mutation-verified log scrubber — spec § 8.1), ordinary behavior gets conventional unit/integration tests, and things automation can't judge (audit-trail readability, reveal-flow security) get named **manual compliance checkpoints** (§ 8.3) with sign-off recorded. Edge-case tests are named `E{n}_*` so coverage of the § 7 table is mechanically greppable.

---

## Industry Best Practices Applied

| Practice | Where it appears |
|----------|------------------|
| PCI DSS scope minimization (no PAN/CVV storage; tokenization; PCI-scoped reveal flow) | spec § 3.1, § 4 note 1, T8; agents.md § 2.1 |
| Immutable, transactional audit trail (append-only table + outbox pattern) | spec § 3.2, T1, T4; agents.md § 2.2 |
| Idempotency keys on all mutations (Stripe-style) | spec § 4 note 4, T3; CLAUDE.md "FinTech-sensitive defaults" |
| Integer minor units for money (ISO 4217) | spec § 4 note 1; agents.md § 1; CLAUDE.md naming (`amountMinor`) |
| Explicit finite state machine for card status | spec § 6 diagram, T2; agents.md § 2.4 |
| Freeze affects new authorizations only (card-network standard) | spec § 4 note 6, E1; agents.md § 2.6 |
| Reconciliation against the processor as source-of-truth divergence control | spec § 3.5, E3, T15, T19 |
| Four-eyes / reason codes for privileged (ops) actions | spec E9, T17; agents.md § 2.5 |
| No-existence-leak authorization (`404` for foreign resources) | spec T7; CLAUDE.md "What to avoid" |
| SLOs as testable requirements, not prose (propagation SLO test, load-test task) | spec § 3.4, T10, T22 |
| RFC 7807 problem details with a stable error-code catalogue | spec § 4 note 5, T5 |
| Compliance sign-off as an explicit deliverable | spec § 8.3, T24 |

## What's in This Folder

```
homework-3/
├── TASKS.md                          # Assignment (read-only)
├── specification.md                  # Layered spec — the graded artifact
├── agents.md                         # AI agent guidelines for the domain
├── CLAUDE.md                         # Editor/AI rules (Claude Code project rules)
├── AI-CONVERSATION.md                # Session log
├── docs/screenshots/                 # Rendered spec + diagram screenshots
└── README.md                         # This file
```
