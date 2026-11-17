import { CustomerSummaryDto } from "../api/customersApi";

export const CustomerCard = ({ customer, onEdit, onDelete }: { customer: CustomerSummaryDto; onEdit: () => void; onDelete: () => void }) => (
  <div className="customer-card">
    <h3>{customer.name}</h3>
    <button onClick={onEdit}>Edit</button>
    <button onClick={onDelete}>Delete</button>
  </div>
);
