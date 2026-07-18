# Skill: Unit Tests — FIRST

Defines the **FIRST** principles every generated unit test must satisfy. The **Unit Test
Generator** MUST apply this skill and confirm compliance in `test-report.md`.

## The principles

| Letter | Principle | Concrete rules for this repo |
|--------|-----------|------------------------------|
| **F** | **Fast** | No network, no filesystem, no `setTimeout`/sleeps, no server startup for pure-logic tests. Whole suite < 2 s. |
| **I** | **Independent** | Each test builds its own state (`beforeEach` + `reset()`); no test reads another test's writes; any subset of tests passes in any order. Environment variables touched by a test are restored afterwards. |
| **R** | **Repeatable** | Deterministic on every machine: no reliance on today's date, locale, timezone, port availability, or randomness. Fixed literal inputs only. |
| **S** | **Self-validating** | Every test ends in strict assertions (`assert/strict`) with exact expected values — no console output inspection, no "look at the log" steps, no floating-point `closeTo` where an exact value is the requirement. |
| **T** | **Timely** | Tests are written for the code changed in the current fix batch, in the same change set. Cover: the exact reported symptom (regression test), the boundary that was wrong, and at least one adjacent negative case. |

## Generator checklist

- [ ] Tests target **only** files listed in `fix-summary.md` (changed code), not the whole app
- [ ] Each seeded bug has a regression test that would **fail on the pre-fix code**
- [ ] Naming: `test("functionName: scenario → expected outcome")`; generated files live in
      `tests/generated/` so they are distinguishable from baseline tests
- [ ] Suite runs with the project's standard command (`npm test`) with no extra setup
- [ ] `test-report.md` states, per FIRST letter, how the generated tests comply
