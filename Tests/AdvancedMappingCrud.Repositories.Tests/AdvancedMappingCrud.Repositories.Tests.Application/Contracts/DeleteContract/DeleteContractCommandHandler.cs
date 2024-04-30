using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts.DeleteContract
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteContractCommandHandler : IRequestHandler<DeleteContractCommand>
    {
        private readonly IContractRepository _contractRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteContractCommandHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteContractCommand request, CancellationToken cancellationToken)
        {
            var contract = await _contractRepository.FindByIdAsync(request.Id, cancellationToken);
            if (contract is null)
            {
                throw new NotFoundException($"Could not find Contract '{request.Id}'");
            }

            _contractRepository.Remove(contract);
        }
    }
}