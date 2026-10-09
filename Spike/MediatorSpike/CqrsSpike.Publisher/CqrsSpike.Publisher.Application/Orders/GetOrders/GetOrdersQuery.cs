using Mediator;

namespace CqrsSpike.Publisher.Application.Orders.GetOrders
{
    public class GetOrdersQuery : IQuery<List<OrderDto>>
    {
        public GetOrdersQuery()
        {
        }
    }
}