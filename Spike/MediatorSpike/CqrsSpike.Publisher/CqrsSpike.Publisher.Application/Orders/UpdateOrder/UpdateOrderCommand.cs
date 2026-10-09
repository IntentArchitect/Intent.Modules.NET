using Mediator;

namespace CqrsSpike.Publisher.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : ICommand
    {
        public UpdateOrderCommand(Guid id, string customerName, decimal total)
        {
            Id = id;
            CustomerName = customerName;
            Total = total;
        }

        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public decimal Total { get; set; }
    }
}