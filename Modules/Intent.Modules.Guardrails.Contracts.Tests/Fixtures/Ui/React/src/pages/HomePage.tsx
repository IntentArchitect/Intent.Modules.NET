import { Link } from "react-router-dom";

export function HomePage() {
  return (
    <section>
      <h1>Welcome</h1>
      <Link to="/customers">Manage customers</Link>
    </section>
  );
}
