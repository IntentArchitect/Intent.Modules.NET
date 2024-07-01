using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.GetOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Order>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Order> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetOrderQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}