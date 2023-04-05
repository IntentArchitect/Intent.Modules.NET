using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithCtors.DeleteEntityWithCtor
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteEntityWithCtorCommandHandler : IRequestHandler<DeleteEntityWithCtorCommand>
    {
        private readonly IEntityWithCtorRepository _entityWithCtorRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteEntityWithCtorCommandHandler(IEntityWithCtorRepository entityWithCtorRepository)
        {
            _entityWithCtorRepository = entityWithCtorRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteEntityWithCtorCommand request, CancellationToken cancellationToken)
        {
            var existingEntityWithCtor = await _entityWithCtorRepository.FindByIdAsync(request.Id, cancellationToken);
            _entityWithCtorRepository.Remove(existingEntityWithCtor);
            return Unit.Value;
        }
    }
}