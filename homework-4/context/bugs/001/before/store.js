// In-memory expense store for SpendLite.
// Dates are "YYYY-MM-DD" strings (lexicographic order == chronological order).

let expenses = [];
let nextId = 1;

export function reset() {
  expenses = [];
  nextId = 1;
}

export function addExpense({ amount, category, date, note }) {
  const expense = {
    id: nextId++,
    amount,
    category,
    date,
    note: note || "",
  };
  expenses.push(expense);
  return expense;
}

export function listExpenses({ from, to, category } = {}) {
  return expenses.filter((e) => {
    if (from && e.date < from) return false;
    if (to && e.date >= to) return false;
    if (category && e.category !== category) return false;
    return true;
  });
}

export function summarize({ from, to } = {}) {
  const filtered = listExpenses({ from, to });
  let total = 0;
  const byCategory = {};
  for (const e of filtered) {
    total += e.amount;
    byCategory[e.category] = (byCategory[e.category] || 0) + e.amount;
  }
  return { count: filtered.length, total, byCategory };
}

export function removeExpense(id) {
  const index = expenses.findIndex((e) => e.id === id);
  if (index === -1) return false;
  expenses.splice(index, 1);
  return true;
}
