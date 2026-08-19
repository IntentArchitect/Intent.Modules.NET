using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMapping.Strict.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ObjectMapping.Strict.Application.Orders.GetOrderById
{
    /// <summary>
    /// Returns a single mapped OrderDto. Pins the non-nullable single Call Site shape (R3.2).
    /// </summary>
    public class GetOrderById : IRequest<OrderDto>, IQuery
    {
        public GetOrderById(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}