using AspNetCoreCleanArchitecture.Sample.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Orders.UpdateOrder
{
    public class UpdateOrderCommand : IRequest, ICommand
    {
        public UpdateOrderCommand(Guid id, Guid buyerId, List<UpdateOrderCommandOrderLinesDto> orderLines)
        {
            Id = id;
            BuyerId = buyerId;
            OrderLines = orderLines;
        }

        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public List<UpdateOrderCommandOrderLinesDto> OrderLines { get; set; }
    }
}