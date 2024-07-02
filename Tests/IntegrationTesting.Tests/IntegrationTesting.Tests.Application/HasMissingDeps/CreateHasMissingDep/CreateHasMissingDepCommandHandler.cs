using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps.CreateHasMissingDep
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateHasMissingDepCommandHandler : IRequestHandler<CreateHasMissingDepCommand, Guid>
    {
        private readonly IHasMissingDepRepository _hasMissingDepRepository;

        [IntentManaged(Mode.Merge)]
        public CreateHasMissingDepCommandHandler(IHasMissingDepRepository hasMissingDepRepository)
        {
            _hasMissingDepRepository = hasMissingDepRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateHasMissingDepCommand request, CancellationToken cancellationToken)
        {
            var hasMissingDep = new HasMissingDep
            {
                Name = request.Name,
                MissingDepId = request.MissingDepId
            };

            _hasMissingDepRepository.Add(hasMissingDep);
            await _hasMissingDepRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return hasMissingDep.Id;
        }
    }
}