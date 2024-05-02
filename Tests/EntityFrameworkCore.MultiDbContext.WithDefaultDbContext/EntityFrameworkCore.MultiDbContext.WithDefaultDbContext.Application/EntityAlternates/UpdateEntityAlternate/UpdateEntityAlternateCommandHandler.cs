using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Common.Exceptions;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.UpdateEntityAlternate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEntityAlternateCommandHandler : IRequestHandler<UpdateEntityAlternateCommand>
    {
        private readonly IEntityAlternateRepository _entityAlternateRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateEntityAlternateCommandHandler(IEntityAlternateRepository entityAlternateRepository)
        {
            _entityAlternateRepository = entityAlternateRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateEntityAlternateCommand request, CancellationToken cancellationToken)
        {
            var entityAlternate = await _entityAlternateRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entityAlternate is null)
            {
                throw new NotFoundException($"Could not find EntityAlternate '{request.Id}'");
            }

            entityAlternate.Message = request.Message;
        }
    }
}