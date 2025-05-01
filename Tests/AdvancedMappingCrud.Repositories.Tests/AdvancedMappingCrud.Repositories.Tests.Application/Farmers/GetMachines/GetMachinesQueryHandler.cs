using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainInvoke;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.GetMachines
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMachinesQueryHandler : IRequestHandler<GetMachinesQuery, List<MachinesDto>>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetMachinesQueryHandler(IFarmerRepository farmerRepository, IMapper mapper)
        {
            _farmerRepository = farmerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MachinesDto>> Handle(GetMachinesQuery request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.FindByIdAsync(request.FarmerId, cancellationToken);
            if (farmer is null)
            {
                throw new NotFoundException($"Could not find Farmer '{request.FarmerId}'");
            }

            var machines = farmer.Machines.Where(x => x.FarmerId == request.FarmerId);
            return machines.MapToMachinesDtoList(_mapper);
        }
    }
}