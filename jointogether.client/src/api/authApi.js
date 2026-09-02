// 1. Add dependencies
import { postJson } from "./httpClient";
import { setToken, clearToken } from "./tokenStorage";
import { saveUser, clearUser } from "./userStorage";

// ---------2. Register function -------------

// 2.1 Send data to API and receive the authentication token and user information.
export async function registerUser({ fullName, email, password }) {
  const result = await postJson("/Auth/register", {
    fullName,
    email,
    password,
  });

  // 2.2 Store the token and user information locally into localStorage.
  setToken(result.token);
  // 2.2 Store the token into localStorage.
  saveUser({ email: result.email, fullName: result.fullName });
  // 2.3 Return the result to the caller.
  return result;
}

// ---------3. Login function -------------
// 3.1 Send email+password to API and receive the authentication token and user information.
export async function loginUser({ email, password }) {
  const result = await postJson("/Auth/login", {
    email,
    password,
  });

  // 3.2 Store the token and user information locally into localStorage.
  setToken(result.token);
  // 3.2 Store the token into localStorage.
  saveUser({ email: result.email, fullName: result.fullName });
  return result;
}

// ---------4. Logout function -------------
export function logoutUser() {
  clearToken();
  clearUser();
}
