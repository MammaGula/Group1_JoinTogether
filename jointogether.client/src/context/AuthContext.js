// AuthContext.js — Authentication state for the whole app.
// ---- Context: Container ----
// ---- Provider: keeps & shares { authed, user, login, logout } with all children ----
// ---- useAuth: custom hook to consume the context easily ----
// Used in: httpClient.js (getToken), any component that needs auth state

import { createContext, useContext, useState } from "react";

const TOKEN_KEY = "jointogether_token";
const USER_KEY = "jointogether_user";

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

function getStoredUser() {
  const saved = localStorage.getItem(USER_KEY);
  return saved ? JSON.parse(saved) : null;
}

// 1. Create the context object.
const AuthContext = createContext(null);

// 2. AuthProvider — wraps the app (in main.jsx)
export function AuthProvider({ children }) {
  // 2.1 authed: is the user currently logged in? (checked from localStorage token)
  const [authed, setAuthed] = useState(!!getToken());

  // 2.2 user: { email, fullName } or null
  const [user, setUser] = useState(getStoredUser());

  // 2.3 login — called after a successful /Auth/login or /Auth/register response
  // token: JWT string from backend
  // userData: { email, fullName } from backend
  const login = (token, userData) => {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(userData));
    setAuthed(true);
    setUser(userData);
  };

  // 2.4 logout — clears all auth data
  const logout = () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setAuthed(false);
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ authed, user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

// 3. useAuth — const { authed, user, login, logout } = useAuth();
export function useAuth() {
  return useContext(AuthContext);
}
