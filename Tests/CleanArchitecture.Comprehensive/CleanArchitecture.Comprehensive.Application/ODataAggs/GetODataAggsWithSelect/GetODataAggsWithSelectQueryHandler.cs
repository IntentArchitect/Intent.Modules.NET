using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Repositories.ODataQuery;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggsWithSelect
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetODataAggsWithSelectQueryHandler : IRequestHandler<GetODataAggsWithSelectQuery, IEnumerable>
    {
        private readonly IODataAggRepository _oDataAggRepository;

        [IntentManaged(Mode.Ignore)]
        public GetODataAggsWithSelectQueryHandler(IODataAggRepository oDataAggRepository)
        {
            _oDataAggRepository = oDataAggRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<IEnumerable> Handle(GetODataAggsWithSelectQuery request, CancellationToken cancellationToken)
        {
            return await _oDataAggRepository.FindAllProjectToWithTransformationAsync(filterExpression: null, transform: request.Transform, cancellationToken: cancellationToken);
        }
    }
}