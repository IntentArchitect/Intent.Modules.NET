using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.CreateEntityDefault
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEntityDefaultCommandHandler : IRequestHandler<CreateEntityDefaultCommand, Guid>
    {
        private readonly IEntityDefaultRepository _entityDefaultRepository;

        [IntentManaged(Mode.Merge)]
        public CreateEntityDefaultCommandHandler(IEntityDefaultRepository entityDefaultRepository)
        {
            _entityDefaultRepository = entityDefaultRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateEntityDefaultCommand request, CancellationToken cancellationToken)
        {
            var entityDefault = new EntityDefault
            {
                Message = request.Message
            };

            _entityDefaultRepository.Add(entityDefault);
            await _entityDefaultRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entityDefault.Id;
        }
    }
}