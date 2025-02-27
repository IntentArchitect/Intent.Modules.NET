using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainInvoke;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.GetFarmers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetFarmersQueryHandler : IRequestHandler<GetFarmersQuery, List<FarmerDto>>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetFarmersQueryHandler(IFarmerRepository farmerRepository, IMapper mapper)
        {
            _farmerRepository = farmerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<FarmerDto>> Handle(GetFarmersQuery request, CancellationToken cancellationToken)
        {
            var farmers = await _farmerRepository.FindAllAsync(cancellationToken);
            return farmers.MapToFarmerDtoList(_mapper);
        }
    }
}