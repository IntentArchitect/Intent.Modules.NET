using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryModels", Version = "1.0")]

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