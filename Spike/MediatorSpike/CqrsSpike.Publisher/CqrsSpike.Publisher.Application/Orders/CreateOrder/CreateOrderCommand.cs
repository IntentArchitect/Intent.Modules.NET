using CqrsSpike.Publisher.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
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