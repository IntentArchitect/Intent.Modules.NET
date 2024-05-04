using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.CreateEntityAlternate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEntityAlternateCommandHandler : IRequestHandler<CreateEntityAlternateCommand, Guid>
    {
        private readonly IEntityAlternateRepository _entityAlternateRepository;

        [IntentManaged(Mode.Merge)]
        public CreateEntityAlternateCommandHandler(IEntityAlternateRepository entityAlternateRepository)
        {
            _entityAlternateRepository = entityAlternateRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateEntityAlternateCommand request, CancellationToken cancellationToken)
        {
            var entityAlternate = new EntityAlternate
            {
                Message = request.Message
            };

            _entityAlternateRepository.Add(entityAlternate);
            await _entityAlternateRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entityAlternate.Id;
        }
    }
}