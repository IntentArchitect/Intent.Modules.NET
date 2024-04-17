using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.GetPetFindByStatus
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPetFindByStatusQueryHandler : IRequestHandler<GetPetFindByStatusQuery, List<Pet>>
    {
        [IntentManaged(Mode.Merge)]
        public GetPetFindByStatusQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<Pet>> Handle(GetPetFindByStatusQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}