using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.Regions.GetRegionById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetRegionByIdQueryHandler : IRequestHandler<GetRegionByIdQuery, RegionDto>
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetRegionByIdQueryHandler(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<RegionDto> Handle(GetRegionByIdQuery request, CancellationToken cancellationToken)
        {
            var region = await _regionRepository.FindByIdAsync(request.Id, cancellationToken);
            if (region is null)
            {
                throw new NotFoundException($"Could not find Region '{request.Id}'");
            }

            return region.MapToRegionDto(_mapper);
        }
    }
}