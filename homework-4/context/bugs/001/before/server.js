// SpendLite HTTP API — zero-dependency Node server.
//
// Routes:
//   GET    /health
//   POST   /expenses            { amount, category, date, note? }
//   GET    /expenses?from&to&category
//   GET    /summary?from&to
//   DELETE /expenses/{id}       (requires X-Admin-Token header)

import http from "node:http";
import { addExpense, listExpenses, summarize, removeExpense } from "./store.js";
import { isAdmin } from "./auth.js";

const PORT = process.env.PORT || 3000;

function json(res, status, body) {
  res.writeHead(status, { "Content-Type": "application/json" });
  res.end(JSON.stringify(body));
}

function readBody(req) {
  return new Promise((resolve, reject) => {
    let data = "";
    req.on("data", (chunk) => (data += chunk));
    req.on("end", () => {
      try {
        resolve(data ? JSON.parse(data) : {});
      } catch {
        reject(new Error("invalid_json"));
      }
    });
  });
}

export const server = http.createServer(async (req, res) => {
  const url = new URL(req.url, `http://localhost:${PORT}`);
  const params = Object.fromEntries(url.searchParams);

  try {
    if (req.method === "GET" && url.pathname === "/health") {
      return json(res, 200, { status: "ok" });
    }

    if (req.method === "POST" && url.pathname === "/expenses") {
      const body = await readBody(req);
      if (!body.category || !body.date) {
        return json(res, 400, { error: "category and date are required" });
      }
      const expense = addExpense({
        amount: Number(body.amount),
        category: body.category,
        date: body.date,
        note: body.note,
      });
      return json(res, 201, expense);
    }

    if (req.method === "GET" && url.pathname === "/expenses") {
      return json(res, 200, listExpenses(params));
    }

    if (req.method === "GET" && url.pathname === "/summary") {
      return json(res, 200, summarize(params));
    }

    const deleteMatch = url.pathname.match(/^\/expenses\/(\d+)$/);
    if (req.method === "DELETE" && deleteMatch) {
      if (!isAdmin(req.headers["x-admin-token"])) {
        return json(res, 404, { error: "not found" });
      }
      const removed = removeExpense(Number(deleteMatch[1]));
      return json(res, removed ? 204 : 404, removed ? undefined : { error: "not found" });
    }

    return json(res, 404, { error: "not found" });
  } catch (err) {
    if (err.message === "invalid_json") {
      return json(res, 400, { error: "invalid JSON body" });
    }
    return json(res, 500, { error: "internal error" });
  }
});

if (process.argv[1] && process.argv[1].endsWith("server.js")) {
  server.listen(PORT, () => {
    console.log(`SpendLite listening on http://localhost:${PORT}`);
  });
}
