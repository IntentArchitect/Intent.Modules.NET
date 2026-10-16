using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryModels", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.GetOrderById
{
    [Authorize]
    public class GetOrderByIdQuery : IQuery<OrderDto>
    {
        public GetOrderByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}