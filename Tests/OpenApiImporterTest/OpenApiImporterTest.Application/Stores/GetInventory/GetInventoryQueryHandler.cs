using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.GetInventory
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, Dictionary<string, int>>
    {
        [IntentManaged(Mode.Merge)]
        public GetInventoryQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Dictionary<string, int>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetInventoryQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}