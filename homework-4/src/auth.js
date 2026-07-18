// Admin authentication for destructive operations.
// The admin token is provided via the ADMIN_TOKEN environment variable;
// when it is unset, admin access is denied entirely (fail closed).

import { timingSafeEqual } from "node:crypto";

export function isAdmin(token) {
  const expected = process.env.ADMIN_TOKEN;
  if (!expected || typeof token !== "string") return false;
  const presented = Buffer.from(token);
  const secret = Buffer.from(expected);
  if (presented.length !== secret.length) return false;
  return timingSafeEqual(presented, secret);
}
