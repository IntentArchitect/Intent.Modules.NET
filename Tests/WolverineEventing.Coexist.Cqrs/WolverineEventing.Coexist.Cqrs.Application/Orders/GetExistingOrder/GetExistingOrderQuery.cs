using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Coexist.Cqrs.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace WolverineEventing.Coexist.Cqrs.Application.Orders.GetExistingOrder
{
    public class GetExistingOrderQuery : IQuery
    {
        public GetExistingOrderQuery(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; set; }
    }
}