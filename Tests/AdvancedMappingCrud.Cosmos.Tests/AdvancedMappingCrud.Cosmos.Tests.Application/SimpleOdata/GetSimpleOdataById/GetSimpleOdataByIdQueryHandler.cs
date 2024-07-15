using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.GetSimpleOdataById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetSimpleOdataByIdQueryHandler : IRequestHandler<GetSimpleOdataByIdQuery, SimpleOdataDto>
    {
        private readonly ISimpleOdataRepository _simpleOdataRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetSimpleOdataByIdQueryHandler(ISimpleOdataRepository simpleOdataRepository, IMapper mapper)
        {
            _simpleOdataRepository = simpleOdataRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<SimpleOdataDto> Handle(GetSimpleOdataByIdQuery request, CancellationToken cancellationToken)
        {
            var simpleOdata = await _simpleOdataRepository.FindByIdAsync(request.Id, cancellationToken);
            if (simpleOdata is null)
            {
                throw new NotFoundException($"Could not find SimpleOdata '{request.Id}'");
            }
            return simpleOdata.MapToSimpleOdataDto(_mapper);
        }
    }
}