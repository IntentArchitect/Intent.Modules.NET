using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets.GetByTags
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetByTagsQueryHandler : IRequestHandler<GetByTagsQuery, List<Pet>>
    {
        [IntentManaged(Mode.Merge)]
        public GetByTagsQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<Pet>> Handle(GetByTagsQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetByTagsQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}