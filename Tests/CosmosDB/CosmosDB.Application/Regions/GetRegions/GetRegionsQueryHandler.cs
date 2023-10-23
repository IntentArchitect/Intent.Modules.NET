using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.Regions.GetRegions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetRegionsQueryHandler : IRequestHandler<GetRegionsQuery, List<RegionDto>>
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetRegionsQueryHandler(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<RegionDto>> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
        {
            var regions = await _regionRepository.FindAllAsync(cancellationToken);
            return regions.MapToRegionDtoList(_mapper);
        }
    }
}