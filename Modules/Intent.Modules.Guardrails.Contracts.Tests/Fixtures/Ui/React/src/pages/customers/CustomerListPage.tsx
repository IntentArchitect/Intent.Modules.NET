import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { customersApi, CustomerSummaryDto } from "../../api/customersApi";
import { CustomerCard } from "../../components/CustomerCard";
import { DeleteCustomerDialog } from "./DeleteCustomerDialog";

export function CustomerListPage() {
  const navigate = useNavigate();
  const [customers, setCustomers] = useState<CustomerSummaryDto[]>([]);
  const [deleting, setDeleting] = useState<CustomerSummaryDto | null>(null);

  const load = () => customersApi.getCustomers().then(setCustomers);

  useEffect(() => {
    load();
  }, []);

  const confirmDelete = async () => {
    if (deleting) {
      await customersApi.deleteCustomer(deleting.id);
      setDeleting(null);
      load();
    }
  };

  return (
    <div>
      <h1>Customers</h1>
      <Link to="/customers/add">Add customer</Link>
      {customers.map((customer) => (
        <CustomerCard
          key={customer.id}
          customer={customer}
          onEdit={() => navigate(`/customers/${customer.id}/edit`)}
          onDelete={() => setDeleting(customer)}
        />
      ))}
      <DeleteCustomerDialog open={deleting !== null} name={deleting?.name} onConfirm={confirmDelete} onClose={() => setDeleting(null)} />
    </div>
  );
}
