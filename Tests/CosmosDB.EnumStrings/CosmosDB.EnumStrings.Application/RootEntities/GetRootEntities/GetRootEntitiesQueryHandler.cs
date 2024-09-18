using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.EnumStrings.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.GetRootEntities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetRootEntitiesQueryHandler : IRequestHandler<GetRootEntitiesQuery, List<RootEntityDto>>
    {
        private readonly IRootEntityRepository _rootEntityRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetRootEntitiesQueryHandler(IRootEntityRepository rootEntityRepository, IMapper mapper)
        {
            _rootEntityRepository = rootEntityRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<RootEntityDto>> Handle(GetRootEntitiesQuery request, CancellationToken cancellationToken)
        {
            var rootEntities = await _rootEntityRepository.FindAllAsync(cancellationToken);
            return rootEntities.MapToRootEntityDtoList(_mapper);
        }
    }
}