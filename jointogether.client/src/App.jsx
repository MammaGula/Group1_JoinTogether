import {AuthProvider} from './context/AuthContext';
import {BrowserRouter, Routes, Route} from 'react-router-dom';
import {AuthPage} from './pages/AuthPage';
import {HomePage} from './pages/HomePage';
import './pages/AuthPage.css';

function AppRoutes() {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <HomePage /> : <AuthPage />;
}


function App() {


  return (
    <AuthProvider>  
      <AppRoutes />
    </AuthProvider>
  )
}

export default App
