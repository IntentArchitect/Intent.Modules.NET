using AspNetCoreCleanArchitecture.Sample.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>, ICommand
    {
        public CreateOrderCommand(string orderNo,
            DateTime orderDate,
            Guid buyerId,
            List<CreateOrderCommandOrderLinesDto> orderLines)
        {
            OrderNo = orderNo;
            OrderDate = orderDate;
            BuyerId = buyerId;
            OrderLines = orderLines;
        }

        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid BuyerId { get; set; }
        public List<CreateOrderCommandOrderLinesDto> OrderLines { get; set; }
    }
}