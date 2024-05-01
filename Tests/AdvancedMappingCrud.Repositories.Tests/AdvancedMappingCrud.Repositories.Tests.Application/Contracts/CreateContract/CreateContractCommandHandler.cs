using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts.CreateContract
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, ContractDto>
    {
        private readonly IContractRepository _contractRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CreateContractCommandHandler(IContractRepository contractRepository, IMapper mapper)
        {
            _contractRepository = contractRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ContractDto> Handle(CreateContractCommand request, CancellationToken cancellationToken)
        {
            var contract = new Contract
            {
                Name = request.Name,
                IsActive = request.IsActive
            };

            _contractRepository.Add(contract);
            await _contractRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return contract.MapToContractDto(_mapper);
        }
    }
}