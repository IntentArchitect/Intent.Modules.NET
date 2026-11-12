import { FormEvent, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { customersApi, CustomerDto } from "../../api/customersApi";

export function CustomerEditPage() {
  const { customerId } = useParams();
  const [customer, setCustomer] = useState<CustomerDto | null>(null);

  useEffect(() => {
    customersApi.getCustomerById(customerId!).then(setCustomer);
  }, [customerId]);

  const save = async (event: FormEvent) => {
    event.preventDefault();
    if (customer) {
      await customersApi.updateCustomer(customer.id, customer);
    }
  };

  if (!customer) {
    return <p>Loading…</p>;
  }

  return (
    <form onSubmit={save}>
      <input value={customer.name} onChange={(e) => setCustomer({ ...customer, name: e.target.value })} />
      <button type="submit">Save</button>
    </form>
  );
}
