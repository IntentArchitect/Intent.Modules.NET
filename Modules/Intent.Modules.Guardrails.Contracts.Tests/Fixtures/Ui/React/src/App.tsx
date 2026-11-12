import { BrowserRouter, Route, Routes } from "react-router-dom";
import { RequireAuth } from "./auth/RequireAuth";
import { Layout } from "./components/Layout";
import { HomePage } from "./pages/HomePage";
import { CustomerAddPage } from "./pages/customers/CustomerAddPage";
import { CustomerEditPage } from "./pages/customers/CustomerEditPage";
import { CustomerListPage } from "./pages/customers/CustomerListPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<Layout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/customers" element={<CustomerListPage />} />
          <Route path="/customers/add" element={<RequireAuth roles={["Admin", "Sales"]}><CustomerAddPage /></RequireAuth>} />
          <Route path="/customers/:customerId/edit" element={<RequireAuth><CustomerEditPage /></RequireAuth>} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
