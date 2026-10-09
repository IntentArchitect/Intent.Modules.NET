using Mediator;

namespace CqrsSpike.Publisher.Application.Orders.GetOrderById
{
    public class GetOrderByIdQuery : IQuery<OrderDto>
    {
        public GetOrderByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}