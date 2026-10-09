using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryModels", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.Orders.GetOrders
{
    public class GetOrdersQuery : IQuery<List<OrderDto>>
    {
        public GetOrdersQuery()
        {
        }
    }
}