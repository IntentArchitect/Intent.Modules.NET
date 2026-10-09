using Mediator;

namespace CqrsSpike.Publisher.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : ICommand<Guid>
    {
        public CreateOrderCommand(string customerName, decimal total)
        {
            CustomerName = customerName;
            Total = total;
        }

        public string CustomerName { get; set; }
        public decimal Total { get; set; }
    }
}