import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

function Navbar() {
  const { user, logout } = useAuth();

  return (
    <nav>
      <Link to="/">Hem</Link>

      {user ? (
        <button onClick={logout}>Logga ut</button>
      ) : (
        <>
          <Link to="/login">Logga in</Link>
          <Link to="/register">Registera</Link>
        </>
      )}
    </nav>
  );
}

export default Navbar;