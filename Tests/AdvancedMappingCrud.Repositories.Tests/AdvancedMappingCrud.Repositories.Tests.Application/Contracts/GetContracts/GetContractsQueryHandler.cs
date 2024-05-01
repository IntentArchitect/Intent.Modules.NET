using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts.GetContracts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetContractsQueryHandler : IRequestHandler<GetContractsQuery, List<ContractDto>>
    {
        private readonly IContractRepository _contractRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetContractsQueryHandler(IContractRepository contractRepository, IMapper mapper)
        {
            _contractRepository = contractRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ContractDto>> Handle(GetContractsQuery request, CancellationToken cancellationToken)
        {
            var contracts = await _contractRepository.FindAllAsync(x => x.IsActive == true, cancellationToken);
            return contracts.MapToContractDtoList(_mapper);
        }
    }
}