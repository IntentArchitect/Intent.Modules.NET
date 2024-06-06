using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.ODataQuery;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs.GetODataAggById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetODataAggByIdQueryHandler : IRequestHandler<GetODataAggByIdQuery, ODataAggDto>
    {
        private readonly IODataAggRepository _oDataAggRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetODataAggByIdQueryHandler(IODataAggRepository oDataAggRepository, IMapper mapper)
        {
            _oDataAggRepository = oDataAggRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ODataAggDto> Handle(GetODataAggByIdQuery request, CancellationToken cancellationToken)
        {
            var oDataAgg = await _oDataAggRepository.FindByIdAsync(request.Id, cancellationToken);
            if (oDataAgg is null)
            {
                throw new NotFoundException($"Could not find ODataAgg '{request.Id}'");
            }

            return oDataAgg.MapToODataAggDto(_mapper);
        }
    }
}