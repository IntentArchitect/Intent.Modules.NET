using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps.DeleteHasMissingDep
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteHasMissingDepCommandHandler : IRequestHandler<DeleteHasMissingDepCommand>
    {
        private readonly IHasMissingDepRepository _hasMissingDepRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteHasMissingDepCommandHandler(IHasMissingDepRepository hasMissingDepRepository)
        {
            _hasMissingDepRepository = hasMissingDepRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteHasMissingDepCommand request, CancellationToken cancellationToken)
        {
            var hasMissingDep = await _hasMissingDepRepository.FindByIdAsync(request.Id, cancellationToken);
            if (hasMissingDep is null)
            {
                throw new NotFoundException($"Could not find HasMissingDep '{request.Id}'");
            }

            _hasMissingDepRepository.Remove(hasMissingDep);
        }
    }
}