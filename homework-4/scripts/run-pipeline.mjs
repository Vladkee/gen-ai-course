#!/usr/bin/env node
// HW4 pipeline runner — single command: `npm run pipeline`
//
// Runs the full agent chain in order via Claude Code headless mode (`claude -p`),
// with an explicit model per stage (see agents/*.agent.md frontmatter). Each agent
// stage instructs the model to load its agent definition and skills itself, so
// skills are loaded automatically — no manual per-agent invocation.
//
// Order (per TASKS.md): Bug Researcher → Research Verifier → Bug Planner →
//                       Bug Fixer → Security Verifier → Unit Test Generator
//
// Prerequisites: Node >= 20, Claude Code CLI installed and authenticated.

import { spawnSync } from "node:child_process";
import { mkdirSync, writeFileSync, existsSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const ROOT = join(dirname(fileURLToPath(import.meta.url)), "..");
const BATCH = process.env.BUG_BATCH || "001";
const BATCH_DIR = `context/bugs/${BATCH}`;
const LOG_DIR = join(ROOT, BATCH_DIR, "logs");

const stages = [
  {
    name: "1-bug-researcher",
    model: "claude-opus-4-8",
    output: `${BATCH_DIR}/research/codebase-research.md`,
    prompt:
      `You are the Bug Researcher of a 4-agent pipeline. Read ${BATCH_DIR}/bug-context.md, ` +
      `then investigate src/ and tests/ to find the root cause of every reported issue. ` +
      `Write ${BATCH_DIR}/research/codebase-research.md containing, per issue: root cause, ` +
      `exact file:line references, quoted code snippets, affected API surface, and suggested ` +
      `fix direction. Do NOT edit any source code — research document only.`,
  },
  {
    name: "2-research-verifier",
    model: "claude-opus-4-8",
    output: `${BATCH_DIR}/research/verified-research.md`,
    prompt:
      `Read agents/research-verifier.agent.md and skills/research-quality-measurement.md, ` +
      `then execute the verifier procedure exactly for batch ${BATCH_DIR}. ` +
      `Deliverable: ${BATCH_DIR}/research/verified-research.md. Do NOT edit source code.`,
  },
  {
    name: "3-bug-planner",
    model: "claude-sonnet-5",
    output: `${BATCH_DIR}/implementation-plan.md`,
    prompt:
      `You are the Bug Planner of a 4-agent pipeline. Read ${BATCH_DIR}/research/verified-research.md. ` +
      `If its Research Quality level is C or F, abort with a clear message. Otherwise write ` +
      `${BATCH_DIR}/implementation-plan.md: per change — target file, exact BEFORE code, exact AFTER ` +
      `code, rationale tied to the verified research, and application order. State the test command ` +
      `(npm test) and any post-fix manual verification. Do NOT edit any source code — plan only.`,
  },
  {
    name: "4-bug-fixer",
    model: "claude-sonnet-5",
    output: `${BATCH_DIR}/fix-summary.md`,
    prompt:
      `Read agents/bug-fixer.agent.md, then execute the fixer procedure exactly for batch ` +
      `${BATCH_DIR}: apply implementation-plan.md change-by-change, run npm test after every ` +
      `change, stop on failure. Deliverable: ${BATCH_DIR}/fix-summary.md.`,
  },
  {
    name: "5-security-verifier",
    model: "claude-opus-4-8",
    output: `${BATCH_DIR}/security-report.md`,
    prompt:
      `Read agents/security-verifier.agent.md, then execute the security review exactly for ` +
      `batch ${BATCH_DIR}, scoped to the files listed in fix-summary.md. ` +
      `Deliverable: ${BATCH_DIR}/security-report.md. Report only — do NOT edit code.`,
  },
  {
    name: "6-unit-test-generator",
    model: "claude-sonnet-5",
    output: `${BATCH_DIR}/test-report.md`,
    prompt:
      `Read agents/unit-test-generator.agent.md and skills/unit-tests-FIRST.md, then execute ` +
      `the generator procedure exactly for batch ${BATCH_DIR}: generate FIRST-compliant tests in ` +
      `tests/generated/ for changed code only, run npm test until green. ` +
      `Deliverable: ${BATCH_DIR}/test-report.md.`,
  },
];

function fail(msg) {
  console.error(`\n✖ ${msg}`);
  process.exit(1);
}

if (!existsSync(join(ROOT, BATCH_DIR, "bug-context.md"))) {
  fail(`${BATCH_DIR}/bug-context.md not found — nothing to run the pipeline on.`);
}
mkdirSync(LOG_DIR, { recursive: true });

console.log(`\n=== HW4 4-agent pipeline — batch ${BATCH} ===\n`);

for (const stage of stages) {
  console.log(`\n──▶ Stage ${stage.name}  (model: ${stage.model})`);
  const result = spawnSync(
    "claude",
    [
      "-p", stage.prompt,
      "--model", stage.model,
      "--permission-mode", "acceptEdits",
    ],
    { cwd: ROOT, shell: true, encoding: "utf8", stdio: ["ignore", "pipe", "pipe"] },
  );

  const log = `# stage: ${stage.name}\n# model: ${stage.model}\n# exit: ${result.status}\n\n` +
    `--- stdout ---\n${result.stdout || ""}\n--- stderr ---\n${result.stderr || ""}`;
  writeFileSync(join(LOG_DIR, `${stage.name}.log`), log);
  process.stdout.write(result.stdout || "");

  if (result.status !== 0) {
    fail(`Stage ${stage.name} exited with code ${result.status} — see ${BATCH_DIR}/logs/${stage.name}.log`);
  }
  if (!existsSync(join(ROOT, stage.output))) {
    fail(`Stage ${stage.name} finished but did not produce ${stage.output} — pipeline stopped.`);
  }
  console.log(`✔ ${stage.name} → ${stage.output}`);
}

console.log(`\n=== Pipeline complete — all artifacts in ${BATCH_DIR}/ ===`);
