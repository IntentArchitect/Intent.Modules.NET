using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.GetSimpleOdata
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetSimpleOdataQueryHandler : IRequestHandler<GetSimpleOdataQuery, List<SimpleOdataDto>>
    {
        private readonly ISimpleOdataRepository _simpleOdataRepository;

        [IntentManaged(Mode.Merge)]
        public GetSimpleOdataQueryHandler(ISimpleOdataRepository simpleOdataRepository)
        {
            _simpleOdataRepository = simpleOdataRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<SimpleOdataDto>> Handle(GetSimpleOdataQuery request, CancellationToken cancellationToken)
        {
            return await _simpleOdataRepository.FindAllProjectToAsync(filterExpression: null, filterProjection: request.Transform, cancellationToken: cancellationToken);
        }
    }
}