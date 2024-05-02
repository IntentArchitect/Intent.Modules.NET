using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Common.Exceptions;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.DeleteEntityDefault
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteEntityDefaultCommandHandler : IRequestHandler<DeleteEntityDefaultCommand>
    {
        private readonly IEntityDefaultRepository _entityDefaultRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteEntityDefaultCommandHandler(IEntityDefaultRepository entityDefaultRepository)
        {
            _entityDefaultRepository = entityDefaultRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteEntityDefaultCommand request, CancellationToken cancellationToken)
        {
            var entityDefault = await _entityDefaultRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entityDefault is null)
            {
                throw new NotFoundException($"Could not find EntityDefault '{request.Id}'");
            }

            _entityDefaultRepository.Remove(entityDefault);
        }
    }
}