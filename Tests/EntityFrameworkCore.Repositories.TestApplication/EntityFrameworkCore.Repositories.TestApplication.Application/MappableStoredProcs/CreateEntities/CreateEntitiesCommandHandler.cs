using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.CreateEntities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEntitiesCommandHandler : IRequestHandler<CreateEntitiesCommand>
    {
        private readonly IMappableStoredProcRepository _mappableStoredProcRepository;

        [IntentManaged(Mode.Merge)]
        public CreateEntitiesCommandHandler(IMappableStoredProcRepository mappableStoredProcRepository)
        {
            _mappableStoredProcRepository = mappableStoredProcRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateEntitiesCommand request, CancellationToken cancellationToken)
        {
            await _mappableStoredProcRepository.CreateEntities(request.Entities
                .Select(e => new EntityRecord(e.Id, e.Name))
                .ToList(), cancellationToken);
        }
    }
}