# CLAUDE.md — Virtual Card Service (FinTech-sensitive defaults)

Project rules for Claude Code (and any AI editor) working in this codebase. Companion to [agents.md](agents.md) (domain rules) and [specification.md](specification.md) (requirements). These rules steer *how* the AI works; agents.md defines *what* it must respect.

## Naming & Patterns

- Endpoints: plural resources, actions as sub-resources (`POST /cards/{id}/freeze`) — never verbs in resource names (`/freezeCard` ❌).
- C#: `PascalCase` public members, `_camelCase` private fields; suffix conventions: `*Controller`, `*Service`, `*Repository`, `*Options` (config).
- Money variables always named with unit: `amountMinor`, never bare `amount` as a decimal.
- Error codes: `snake_case`, past-tense/state style (`card_frozen`, `reveal_token_expired`) matching the catalogue in spec § 4.5; never invent codes ad hoc — extend the catalogue first.
- Tests: `MethodName_Scenario_ExpectedOutcome`; edge-case tests prefixed `E{n}_`.

## FinTech-Sensitive Defaults (apply even when the prompt doesn't mention them)

- New mutation endpoint? Add `Idempotency-Key` support, ownership check, audit event, and rate-limit registration **by default** — do not wait to be asked.
- New log statement? Assume it will be read by an auditor: structured, no payloads from card endpoints, no secrets, no token values.
- New query? Paginate by default (cursor, default 25 / max 100); unbounded list queries are a review blocker.
- Monetary math? Integer minor units, same-currency guard first line.
- Anything touching `audit_events`? Read-and-append only; generating an UPDATE/DELETE against it is always a bug.

## What to Avoid

- ❌ Floats/doubles for money; `DateTime.Now` (use `DateTimeOffset.UtcNow` via injected clock)
- ❌ Catch-and-continue around processor calls — failures must surface to the retry/reconciliation machinery
- ❌ Returning processor error bodies to end users
- ❌ `403` on other users' resources (use `404`); `404` on empty collections (use `200 []`)
- ❌ Test data containing realistic PANs — use the documented test tokens only
- ❌ Committing generated OpenAPI or migration files without regenerating them from source in the same commit

## When Unsure

Prefer asking over assuming for: state-machine changes, audit schema changes, anything in the reveal-token flow, retention/deletion of any data. For everything else, follow the spec's implementation notes (§ 4) and flag the assumption in the PR description.
