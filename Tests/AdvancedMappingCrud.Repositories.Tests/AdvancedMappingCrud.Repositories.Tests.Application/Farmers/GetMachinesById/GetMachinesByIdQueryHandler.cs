using System;
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

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.GetMachinesById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMachinesByIdQueryHandler : IRequestHandler<GetMachinesByIdQuery, MachinesDto>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetMachinesByIdQueryHandler(IFarmerRepository farmerRepository, IMapper mapper)
        {
            _farmerRepository = farmerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MachinesDto> Handle(GetMachinesByIdQuery request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.FindByIdAsync(request.FarmerId, cancellationToken);
            if (farmer is null)
            {
                throw new NotFoundException($"Could not find Farmer '{request.FarmerId}'");
            }

            var machines = farmer.Machines.FirstOrDefault(x => x.Id == request.Id && x.FarmerId == request.FarmerId);
            if (machines is null)
            {
                throw new NotFoundException($"Could not find Machines '({request.Id}, {request.FarmerId})'");
            }
            return machines.MapToMachinesDto(_mapper);
        }
    }
}