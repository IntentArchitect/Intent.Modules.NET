using CqrsSpike.Subscriber.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.OrderRecords.RecordOrder
{
    public class RecordOrderCommand : IRequest, ICommand
    {
        public RecordOrderCommand(Guid orderId, string customerName, decimal total)
        {
            OrderId = orderId;
            CustomerName = customerName;
            Total = total;
        }

        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal Total { get; set; }
    }
}