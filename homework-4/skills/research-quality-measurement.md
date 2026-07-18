# Skill: Research Quality Measurement

Defines how the **Bug Research Verifier** measures and reports the quality of Bug Researcher
output (`research/codebase-research.md`). The verifier MUST apply this skill when writing
`research/verified-research.md`.

## Quality levels

Assign exactly one level. Reference accuracy = share of file:line references that point at the
claimed code, with quoted snippets matching the source (whitespace-insensitive).

| Level | Label | Criteria (all must hold) | Planner may proceed? |
|-------|-------|--------------------------|----------------------|
| **A** | Verified-Accurate | 100% reference accuracy; root cause identified for every reported symptom; no claim contradicted by source | ✅ Yes |
| **B** | Minor Discrepancies | ≥ 90% reference accuracy; discrepancies are cosmetic (line drift ≤ 3 lines, harmless paraphrase) and do **not** change any conclusion; root causes still correct | ✅ Yes — verifier corrects references inline |
| **C** | Materially Flawed | < 90% reference accuracy, **or** at least one root cause wrong/unsupported, **or** a reported symptom left unexplained | ⛔ No — re-research required; verifier lists what to redo |
| **F** | Unusable | References largely unresolvable, fabricated code, or research addresses the wrong codebase/symptoms | ⛔ No — restart research from bug-context |

## Measurement procedure

1. Open every file referenced by the research at the cited lines; compare cited snippets against
   the actual source.
2. Score reference accuracy (verified references ÷ total references).
3. For each reported symptom in `bug-context.md`, check the research names a root cause and that
   the cited code can actually produce the symptom.
4. Record every discrepancy: what the research claims vs. what the source shows.
5. Assign the level using the table above — when in doubt between two levels, assign the lower one.

## Required structure of `verified-research.md`

1. **Verification Summary** — overall pass/fail, Research Quality level (per this skill), one-line rationale
2. **Verified Claims** — table: claim, file:line, verification result
3. **Discrepancies Found** — numbered list (or "None")
4. **Research Quality Assessment** — level + reasoning against the criteria above
5. **References** — every file inspected during verification
