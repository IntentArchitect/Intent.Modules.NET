using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.CreateEntityAppDefault
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEntityAppDefaultCommandHandler : IRequestHandler<CreateEntityAppDefaultCommand, Guid>
    {
        private readonly IEntityAppDefaultRepository _entityAppDefaultRepository;

        [IntentManaged(Mode.Merge)]
        public CreateEntityAppDefaultCommandHandler(IEntityAppDefaultRepository entityAppDefaultRepository)
        {
            _entityAppDefaultRepository = entityAppDefaultRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateEntityAppDefaultCommand request, CancellationToken cancellationToken)
        {
            var entityAppDefault = new EntityAppDefault
            {
                Message = request.Message
            };

            _entityAppDefaultRepository.Add(entityAppDefault);
            await _entityAppDefaultRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entityAppDefault.Id;
        }
    }
}