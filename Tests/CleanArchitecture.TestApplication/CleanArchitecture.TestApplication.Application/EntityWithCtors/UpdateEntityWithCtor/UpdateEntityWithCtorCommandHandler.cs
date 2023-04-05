using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithCtors.UpdateEntityWithCtor
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEntityWithCtorCommandHandler : IRequestHandler<UpdateEntityWithCtorCommand>
    {
        private readonly IEntityWithCtorRepository _entityWithCtorRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateEntityWithCtorCommandHandler(IEntityWithCtorRepository entityWithCtorRepository)
        {
            _entityWithCtorRepository = entityWithCtorRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateEntityWithCtorCommand request, CancellationToken cancellationToken)
        {
            var existingEntityWithCtor = await _entityWithCtorRepository.FindByIdAsync(request.Id, cancellationToken);
            existingEntityWithCtor.Name = request.Name;
            return Unit.Value;
        }
    }
}