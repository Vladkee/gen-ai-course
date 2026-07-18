// Baseline tests written alongside the initial implementation.
// (These pre-date the pipeline run; generated tests live in tests/generated/.)

import { test, beforeEach } from "node:test";
import assert from "node:assert/strict";
import { reset, addExpense, listExpenses, summarize, removeExpense } from "../src/store.js";

beforeEach(() => reset());

test("addExpense assigns sequential ids", () => {
  const a = addExpense({ amount: 10, category: "food", date: "2026-07-01" });
  const b = addExpense({ amount: 20, category: "travel", date: "2026-07-02" });
  assert.equal(a.id, 1);
  assert.equal(b.id, 2);
});

test("listExpenses filters by category", () => {
  addExpense({ amount: 10, category: "food", date: "2026-07-01" });
  addExpense({ amount: 20, category: "travel", date: "2026-07-02" });
  const result = listExpenses({ category: "food" });
  assert.equal(result.length, 1);
  assert.equal(result[0].category, "food");
});

test("listExpenses 'from' bound is inclusive", () => {
  addExpense({ amount: 10, category: "food", date: "2026-07-01" });
  addExpense({ amount: 20, category: "food", date: "2026-07-05" });
  const result = listExpenses({ from: "2026-07-05" });
  assert.equal(result.length, 1);
  assert.equal(result[0].date, "2026-07-05");
});

test("summarize totals whole-number amounts per category", () => {
  addExpense({ amount: 10, category: "food", date: "2026-07-01" });
  addExpense({ amount: 20, category: "food", date: "2026-07-02" });
  addExpense({ amount: 5, category: "travel", date: "2026-07-03" });
  const summary = summarize();
  assert.equal(summary.count, 3);
  assert.equal(summary.total, 35);
  assert.equal(summary.byCategory.food, 30);
  assert.equal(summary.byCategory.travel, 5);
});

test("removeExpense removes an existing expense", () => {
  const e = addExpense({ amount: 10, category: "food", date: "2026-07-01" });
  assert.equal(removeExpense(e.id), true);
  assert.equal(listExpenses().length, 0);
});

test("removeExpense returns false for unknown id", () => {
  assert.equal(removeExpense(999), false);
});
