using CqrsSpike.Publisher.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
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