using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.GetStoreOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetStoreOrderQueryHandler : IRequestHandler<GetStoreOrderQuery, Order>
    {
        [IntentManaged(Mode.Merge)]
        public GetStoreOrderQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Order> Handle(GetStoreOrderQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}