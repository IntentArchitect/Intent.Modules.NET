import { Link, Outlet } from "react-router-dom";

export function Layout() {
  return (
    <div className="app">
      <nav>
        <Link to="/">Home</Link>
        <Link to="/customers">Customers</Link>
      </nav>
      <main>
        <Outlet />
      </main>
    </div>
  );
}
