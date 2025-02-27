using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainInvoke;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.GetFarmerById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetFarmerByIdQueryHandler : IRequestHandler<GetFarmerByIdQuery, FarmerDto>
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetFarmerByIdQueryHandler(IFarmerRepository farmerRepository, IMapper mapper)
        {
            _farmerRepository = farmerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<FarmerDto> Handle(GetFarmerByIdQuery request, CancellationToken cancellationToken)
        {
            var farmer = await _farmerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (farmer is null)
            {
                throw new NotFoundException($"Could not find Farmer '{request.Id}'");
            }
            return farmer.MapToFarmerDto(_mapper);
        }
    }
}