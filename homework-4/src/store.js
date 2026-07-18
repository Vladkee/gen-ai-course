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
    if (to && e.date > to) return false;
    if (category && e.category !== category) return false;
    return true;
  });
}

export function summarize({ from, to } = {}) {
  const filtered = listExpenses({ from, to });
  let totalCents = 0;
  const centsByCategory = {};
  for (const e of filtered) {
    const cents = Math.round(e.amount * 100);
    totalCents += cents;
    centsByCategory[e.category] = (centsByCategory[e.category] || 0) + cents;
  }
  const byCategory = {};
  for (const [category, cents] of Object.entries(centsByCategory)) {
    byCategory[category] = cents / 100;
  }
  return { count: filtered.length, total: totalCents / 100, byCategory };
}

export function removeExpense(id) {
  const index = expenses.findIndex((e) => e.id === id);
  if (index === -1) return false;
  expenses.splice(index, 1);
  return true;
}
