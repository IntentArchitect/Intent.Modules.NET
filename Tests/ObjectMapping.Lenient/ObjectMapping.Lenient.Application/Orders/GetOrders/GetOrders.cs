using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMapping.Lenient.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders.GetOrders
{
    /// <summary>
    /// Returns every order as a collection of mapped DTOs. Pins the List Call Site shape (R3.3).
    /// </summary>
    public class GetOrders : IRequest<List<OrderDto>>, IQuery
    {
        public GetOrders()
        {
        }
    }
}