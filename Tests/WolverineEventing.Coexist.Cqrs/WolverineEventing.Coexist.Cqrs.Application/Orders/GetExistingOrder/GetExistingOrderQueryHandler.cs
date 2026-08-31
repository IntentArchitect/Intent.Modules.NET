using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace WolverineEventing.Coexist.Cqrs.Application.Orders.GetExistingOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetExistingOrderQueryHandler
    {
        [IntentManaged(Mode.Merge)]
        public GetExistingOrderQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<OrderDto> Handle(GetExistingOrderQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new OrderDto { Field = request.OrderId.ToString() };
        }
    }
}