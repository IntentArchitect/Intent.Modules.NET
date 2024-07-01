using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.GetByStatuses
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetByStatusesQueryHandler : IRequestHandler<GetByStatusesQuery, List<Pet>>
    {
        [IntentManaged(Mode.Merge)]
        public GetByStatusesQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<Pet>> Handle(GetByStatusesQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetByStatusesQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}