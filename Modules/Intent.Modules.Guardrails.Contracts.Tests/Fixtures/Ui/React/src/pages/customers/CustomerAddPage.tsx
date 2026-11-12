import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCreateCustomerMutation } from "../../api/customersHooks";

export function CustomerAddPage() {
  const navigate = useNavigate();
  const [name, setName] = useState("");
  const [createCustomer, { isLoading }] = useCreateCustomerMutation();

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    await createCustomer({ name }).unwrap();
    navigate("/customers");
  };

  return (
    <form onSubmit={submit}>
      <input value={name} onChange={(e) => setName(e.target.value)} />
      <button type="submit" disabled={isLoading}>Save</button>
    </form>
  );
}
