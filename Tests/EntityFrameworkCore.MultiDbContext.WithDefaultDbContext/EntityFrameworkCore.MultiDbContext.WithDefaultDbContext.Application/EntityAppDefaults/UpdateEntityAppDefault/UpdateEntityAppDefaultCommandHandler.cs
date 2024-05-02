using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Common.Exceptions;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.UpdateEntityAppDefault
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEntityAppDefaultCommandHandler : IRequestHandler<UpdateEntityAppDefaultCommand>
    {
        private readonly IEntityAppDefaultRepository _entityAppDefaultRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateEntityAppDefaultCommandHandler(IEntityAppDefaultRepository entityAppDefaultRepository)
        {
            _entityAppDefaultRepository = entityAppDefaultRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateEntityAppDefaultCommand request, CancellationToken cancellationToken)
        {
            var entityAppDefault = await _entityAppDefaultRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entityAppDefault is null)
            {
                throw new NotFoundException($"Could not find EntityAppDefault '{request.Id}'");
            }

            entityAppDefault.Message = request.Message;
        }
    }
}