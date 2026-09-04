// - Thin wrapper around fetch for talking to the JoinTogether API.
// - The base URL comes from the environment so it can point at a local
// - dotnet run instance in development and a deployed API in production.
import { getToken } from "../context/AuthContext";

const BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "https://localhost:7211/api";

/**
 * Sends a JSON request and returns the parsed response body.
 * Throws an ApiError with a user-facing message when the API responds
 * with a non-2xx status, so callers can show it directly in the form.
 */

async function sendJson(method, path, body) {
  const headers = {};
  if (body !== undefined) headers["Content-Type"] = "application/json";
  const token = getToken();
  if (token) headers["Authorization"] = `Bearer ${token}`;

  let response;
  try {
    response = await fetch(`${BASE_URL}${path}`, {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined,
    });
  } catch {
    throw new ApiError(
      "Could not reach the server. Check your connection and try again.",
    );
  }

  const isJson = response.headers
    .get("content-type")
    ?.includes("application/json");
  const data = isJson ? await response.json() : null;

  if (!response.ok) {
    throw new ApiError(extractErrorMessage(data));
  }

  return data;
}

export function getJson(path) {
  return sendJson("GET", path);
}

export function postJson(path, body) {
  return sendJson("POST", path, body);
}

export function putJson(path, body) {
  return sendJson("PUT", path, body);
}

export function deleteJson(path) {
  return sendJson("DELETE", path);
}

// Handles both the API's hand-rolled `{ message }` error shape and the
// `{ errors: { Field: ["..."] } }` shape ASP.NET Core's [ApiController]
// generates automatically for model validation failures.
function extractErrorMessage(data) {
  if (data?.message) return data.message;

  const firstFieldErrors = data?.errors && Object.values(data.errors)[0];
  if (firstFieldErrors?.[0]) return firstFieldErrors[0];

  return "Something went wrong. Please try again.";
}

export class ApiError extends Error {}
