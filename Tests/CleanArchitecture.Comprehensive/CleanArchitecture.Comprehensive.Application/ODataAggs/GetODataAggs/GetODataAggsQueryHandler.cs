using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Repositories.ODataQuery;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetODataAggsQueryHandler : IRequestHandler<GetODataAggsQuery, List<ODataAggDto>>
    {
        private readonly IODataAggRepository _oDataAggRepository;

        [IntentManaged(Mode.Ignore)]
        public GetODataAggsQueryHandler(IODataAggRepository oDataAggRepository)
        {
            _oDataAggRepository = oDataAggRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ODataAggDto>> Handle(GetODataAggsQuery request, CancellationToken cancellationToken)
        {
            return await _oDataAggRepository.FindAllProjectToAsync(filterExpression: null, filterProjection: request.Transform, cancellationToken: cancellationToken);
        }
    }
}