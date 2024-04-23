using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.GetStoreInventory
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetStoreInventoryQueryHandler : IRequestHandler<GetStoreInventoryQuery, Dictionary<string, int>>
    {
        [IntentManaged(Mode.Merge)]
        public GetStoreInventoryQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Dictionary<string, int>> Handle(
            GetStoreInventoryQuery request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}