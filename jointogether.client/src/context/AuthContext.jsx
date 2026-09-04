import { createContext, useContext, useMemo, useState } from "react";
import { loginUser, registerUser } from "../api/authApi";

const TOKEN_KEY = "jointogether_token";
const USER_KEY = "jointogether_user";

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

function getStoredUser() {
  const saved = localStorage.getItem(USER_KEY);
  return saved ? JSON.parse(saved) : null;
}

function persistSession({ token, email, fullName }) {
  const userData = { email, fullName };
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(USER_KEY, JSON.stringify(userData));
  return userData;
}

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [authed, setAuthed] = useState(!!getToken());
  const [user, setUser] = useState(getStoredUser());

  const login = async (credentials) => {
    const response = await loginUser(credentials);
    const userData = persistSession(response);
    setAuthed(true);
    setUser(userData);
    return response;
  };

  const register = async (credentials) => {
    const response = await registerUser(credentials);
    const userData = persistSession(response);
    setAuthed(true);
    setUser(userData);
    return response;
  };

  const logout = () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setAuthed(false);
    setUser(null);
  };

  const value = useMemo(
    () => ({ authed, user, login, register, logout }),
    [authed, user],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  return useContext(AuthContext);
}
