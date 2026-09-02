// - Manages the storage and retrieval of the authenticated user's information in localStorage.
// - Keep the user's information in sync with the authentication token.

const USER_KEY = "jointogether_user";

// Returns { email, fullName } from the last login/register response, or null.
export function getUser() {
  const saved = localStorage.getItem(USER_KEY);
  return saved ? JSON.parse(saved) : null;
}

export function saveUser(user) {
  localStorage.setItem(USER_KEY, JSON.stringify(user));
}

export function clearUser() {
  localStorage.removeItem(USER_KEY);
}
