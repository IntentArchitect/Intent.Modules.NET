using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMappingTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders.GetOrderSummaryById
{
    public class GetOrderSummaryById : IRequest<OrderSummaryDto>, IQuery
    {
        public GetOrderSummaryById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}