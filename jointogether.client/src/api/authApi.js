// authApi.js — Auth endpoint calls only.
// - Talks to the backend via httpClient.js (fetch wrapper that already
//   attaches the Bearer token and parses JSON/errors for us).


import { postJson } from "./httpClient";

// 1. Register — POST /Auth/register
// body: { fullName, email, password }
// Returns: { token, email, fullName }
export function registerUser({ fullName, email, password }) {
  return postJson("/Auth/register", { fullName, email, password });
}

// 2. Login — POST /Auth/login
// body: { email, password }
// Returns: { token, email, fullName }
export function loginUser({ email, password }) {
  return postJson("/Auth/login", { email, password });
}
