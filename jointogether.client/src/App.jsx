import { AuthProvider, useAuth } from "./context/AuthContext";
import {
  BrowserRouter,
  Navigate,
  Route,
  Routes,
  useParams,
} from "react-router-dom";
import { AuthPage } from "./pages/AuthPage";
import { HomePage } from "./pages/HomePage";
import "./pages/AuthPage.css";
import "./pages/QuizPage.css";
import { QuizPage } from "./pages/QuizPage";
import Navbar from "./components/Navbar";
import "./pages/HomePage.css";
import { CreateActivityPage } from "./pages/CreateActivityPage";

// Keys QuizPage on locationId so navigating from one location's quiz to
// another (a param-only route change) fully remounts it instead of leaving
// stale quiz-progress state behind.
function QuizPageRoute() {
  const { locationId } = useParams();
  return <QuizPage key={locationId} />; // Ensures QuizPage remounts when locationId changes
}

function AppRoutes() {
  const { authed } = useAuth();

  return (
    <BrowserRouter>
      {/* <Navbar /> */}
      <Routes>
        <Route path="/" element={authed ? <HomePage /> : <AuthPage />} />
        <Route
          path="*"
          element={authed ? <Navigate to="/" replace /> : <AuthPage />}
        />
        <Route
          path="/quiz/:locationId"
          element={authed ? <QuizPageRoute /> : <Navigate to="/" replace />}
        />
        <Route
  path="/activities/new"
  element={authed ? <CreateActivityPage /> : <Navigate to="/" replace />}
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
  );
}

export default App;
