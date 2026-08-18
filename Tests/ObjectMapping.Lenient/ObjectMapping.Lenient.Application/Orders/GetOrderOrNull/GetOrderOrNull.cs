using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMapping.Lenient.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders.GetOrderOrNull
{
    /// <summary>
    /// Returns a mapped OrderDto or null when no order matches. Pins the null-conditional Call Site shape (R3.4).
    /// </summary>
    public class GetOrderOrNull : IRequest<OrderDto?>, IQuery
    {
        public GetOrderOrNull(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}