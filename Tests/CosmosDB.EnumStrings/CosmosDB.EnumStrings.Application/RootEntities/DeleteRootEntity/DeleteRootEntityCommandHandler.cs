using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EnumStrings.Domain.Common.Exceptions;
using CosmosDB.EnumStrings.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.DeleteRootEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteRootEntityCommandHandler : IRequestHandler<DeleteRootEntityCommand>
    {
        private readonly IRootEntityRepository _rootEntityRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteRootEntityCommandHandler(IRootEntityRepository rootEntityRepository)
        {
            _rootEntityRepository = rootEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteRootEntityCommand request, CancellationToken cancellationToken)
        {
            var rootEntity = await _rootEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (rootEntity is null)
            {
                throw new NotFoundException($"Could not find RootEntity '{request.Id}'");
            }

            _rootEntityRepository.Remove(rootEntity);
        }
    }
}