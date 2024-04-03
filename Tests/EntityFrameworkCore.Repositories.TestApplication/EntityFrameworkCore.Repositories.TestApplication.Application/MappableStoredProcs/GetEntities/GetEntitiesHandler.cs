using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.GetEntities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEntitiesHandler : IRequestHandler<GetEntities, List<EntityDto>>
    {
        private readonly IMappableStoredProcRepository _mappableStoredProcRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetEntitiesHandler(IMappableStoredProcRepository mappableStoredProcRepository, IMapper mapper)
        {
            _mappableStoredProcRepository = mappableStoredProcRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<EntityDto>> Handle(GetEntities request, CancellationToken cancellationToken)
        {
            var result = await _mappableStoredProcRepository.GetEntities(cancellationToken);
            return result.MapToEntityDtoList(_mapper);
        }
    }
}