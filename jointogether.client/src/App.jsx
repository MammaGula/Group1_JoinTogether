import { AuthProvider, useAuth } from './context/AuthContext';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AuthPage } from './pages/AuthPage';
import { HomePage } from './pages/HomePage';
import './pages/AuthPage.css';

function AppRoutes() {
  const { authed } = useAuth();

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={authed ? <HomePage /> : <AuthPage />} />
        <Route
          path="*"
          element={authed ? <Navigate to="/" replace /> : <AuthPage />}
        />
      </Routes>
    </BrowserRouter>
  );
}


function App() {


  return (
    <AuthProvider>  
      <AppRoutes />
    </AuthProvider>
  )
}

export default App
