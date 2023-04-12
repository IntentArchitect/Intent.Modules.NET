using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithCtors.CreateEntityWithCtor
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEntityWithCtorCommandHandler : IRequestHandler<CreateEntityWithCtorCommand, Guid>
    {
        private readonly IEntityWithCtorRepository _entityWithCtorRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateEntityWithCtorCommandHandler(IEntityWithCtorRepository entityWithCtorRepository)
        {
            _entityWithCtorRepository = entityWithCtorRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateEntityWithCtorCommand request, CancellationToken cancellationToken)
        {
            var newEntityWithCtor = new EntityWithCtor();

            _entityWithCtorRepository.Add(newEntityWithCtor);
            await _entityWithCtorRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newEntityWithCtor.Id;
        }
    }
}