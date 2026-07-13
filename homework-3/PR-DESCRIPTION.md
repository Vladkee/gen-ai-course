## Summary

Implemented Homework 3 as a **specification package** for a **virtual card lifecycle** feature (create, freeze/unfreeze, spending limits, transaction visibility) in a regulated FinTech environment. Documents only — no code, per assignment. The centerpiece `specification.md` is a layered spec designed to be executable by an engineering team or AI agent without guessing.

> Note: this branch is stacked on `homework-2-submission` (the `.agents/` system lives there); once PR #2 merges, this diff reduces to HW3 files + agent-system fixes only.

## Deliverables

| File | Content | Status |
|------|---------|--------|
| `homework-3/specification.md` | Layered spec: high-level objective → 6 mid-level objectives → non-functional/policy targets → implementation notes → beginning/ending context → 24 low-level tasks | ✅ Required |
| `homework-3/agents.md` | AI agent guidelines: tech stack, banking domain rules, testing expectations, edge-case treatment | ✅ Required |
| `homework-3/.cursor/rules/fintech-virtual-cards.md` | Editor/AI rules: naming, patterns, FinTech-sensitive defaults, anti-patterns | ✅ Required |
| `homework-3/README.md` | Student info, rationale (incl. performance-target derivation), industry best practices with section references | ✅ Required |

## What Was Built

- **Layered specification** with explicit traceability: every low-level task maps to a mid-level objective; a traceability summary table (§ 10) closes the loop objectives → tasks → edge cases → verification
- **16-row edge-case & failure-mode table** (§ 7), each with user-visible outcome *and* audit/compliance implication (freeze vs. in-flight authorizations, store-vs-processor divergence, reveal-token replay, ops-frozen guard…)
- **Verification as a first-class layer** (§ 8): per-objective verification methods, test categories with fixtures, and manual compliance checkpoints with sign-off
- **Measurable performance targets** (§ 3.4), labeled as assumed and justified per operation (e.g. freeze ≤ 500 ms API but the real promise is the ≤ 5 s authorization-decline propagation SLO)
- **Mermaid card-lifecycle state diagram** (§ 6) treated as a closed state machine with defined error semantics
- **First commit fixes the agent system itself**: status-table drift (AI-SUMMARY.md is now the single source of truth), a docs-only workflow variant, completed `generate-pr` Step 7, clarified `read-tasks` markers

## AI Tools Used

| Phase | Tool | Model | Mode |
|-------|------|-------|------|
| Agent-system review & fixes | Claude Code | Claude Fable 5 | CLI (Agent) |
| Specification & docs authoring | Claude Code | Claude Fable 5 | CLI (Agent) |

> ⚠️ **MODEL CHANGE** vs. HW2: `Claude Sonnet 4.5 (copilot)` → `Claude Fable 5 (Claude Code CLI)`

See [AI-CONVERSATION.md](https://github.com/Vladkee/gen-ai-course/blob/homework-3-submission/homework-3/AI-CONVERSATION.md) for the full session log.

## Challenges

1. **Branching from `main` silently dropped the agent system**: `.agents/`, `.github/`, and `AI-SUMMARY.md` were never merged to main — resolved by stacking the HW3 branch on `homework-2-submission`
2. **Code-centric workflow didn't fit a docs-only homework**: quality gates demanded `start.ps1` and endpoint tables — resolved by adding a docs-only variant to `homework-workflow.instructions.md` before writing the spec

## How to Review

No run steps — documents only. Suggested reading order:

1. `homework-3/README.md` — rationale and best-practices map
2. `homework-3/specification.md` — §2 objectives → §7 edge cases → §9 tasks → §10 traceability
3. `homework-3/agents.md` and `.cursor/rules/fintech-virtual-cards.md`

## Screenshots

### AI collaboration — agent-system review findings
![Agent system review](https://raw.githubusercontent.com/Vladkee/gen-ai-course/homework-3-submission/homework-3/docs/screenshots/claude-code-agent-system-review.png)

### AI collaboration — instruction fixes applied
![Instruction fixes](https://raw.githubusercontent.com/Vladkee/gen-ai-course/homework-3-submission/homework-3/docs/screenshots/claude-code-instruction-fixes.png)

### AI collaboration — HW3 planning session
![Planning session](https://raw.githubusercontent.com/Vladkee/gen-ai-course/homework-3-submission/homework-3/docs/screenshots/claude-code-session-planning.png)
