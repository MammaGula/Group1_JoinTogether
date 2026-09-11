import { AuthProvider, useAuth } from './context/AuthContext';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AuthPage } from './pages/AuthPage';
import { HomePage } from './pages/HomePage';
import './pages/AuthPage.css';
import './pages/QuizPage.css';
import { QuizPage } from './pages/QuizPage';          
import Navbar from './components/Navbar';
import './pages/HomePage.css';

function AppRoutes() {
  const { authed } = useAuth();

  return (
    <BrowserRouter>
      {/* <Navbar /> */}
      <Routes>
        <Route path="/" element={authed ? <HomePage /> : <AuthPage /> } />
        <Route
          path="*"
          element={authed ? <Navigate to="/" replace /> : <AuthPage />}
        />
        <Route path="/quiz/:locationId" element={authed ? <QuizPage /> : <Navigate to="/" replace />} />
     

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
