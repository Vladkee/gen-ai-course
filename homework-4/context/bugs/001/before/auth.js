// Admin authentication for destructive operations.

const ADMIN_TOKEN = "spendlite-admin-2026";

export function isAdmin(token) {
  console.log("[auth] admin check, token=" + token);
  return token == ADMIN_TOKEN;
}
