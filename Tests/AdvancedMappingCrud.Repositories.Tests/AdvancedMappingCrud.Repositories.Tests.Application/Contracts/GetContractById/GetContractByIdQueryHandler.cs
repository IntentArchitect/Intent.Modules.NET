using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts.GetContractById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetContractByIdQueryHandler : IRequestHandler<GetContractByIdQuery, ContractDto>
    {
        private readonly IContractRepository _contractRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetContractByIdQueryHandler(IContractRepository contractRepository, IMapper mapper)
        {
            _contractRepository = contractRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ContractDto> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
        {
            var contract = await _contractRepository.FindByIdAsync(request.Id, cancellationToken);
            if (contract is null)
            {
                throw new NotFoundException($"Could not find Contract '{request.Id}'");
            }
            return contract.MapToContractDto(_mapper);
        }
    }
}