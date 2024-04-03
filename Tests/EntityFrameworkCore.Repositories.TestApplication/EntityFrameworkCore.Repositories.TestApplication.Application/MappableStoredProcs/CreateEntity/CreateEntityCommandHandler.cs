using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.CreateEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEntityCommandHandler : IRequestHandler<CreateEntityCommand>
    {
        private readonly IMappableStoredProcRepository _mappableStoredProcRepository;

        [IntentManaged(Mode.Merge)]
        public CreateEntityCommandHandler(IMappableStoredProcRepository mappableStoredProcRepository)
        {
            _mappableStoredProcRepository = mappableStoredProcRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateEntityCommand request, CancellationToken cancellationToken)
        {
            await _mappableStoredProcRepository.CreateEntity(request.Id, request.Name, cancellationToken);
        }
    }
}