using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EnumStrings.Domain;
using CosmosDB.EnumStrings.Domain.Common.Exceptions;
using CosmosDB.EnumStrings.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.UpdateRootEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateRootEntityCommandHandler : IRequestHandler<UpdateRootEntityCommand>
    {
        private readonly IRootEntityRepository _rootEntityRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateRootEntityCommandHandler(IRootEntityRepository rootEntityRepository)
        {
            _rootEntityRepository = rootEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateRootEntityCommand request, CancellationToken cancellationToken)
        {
            var rootEntity = await _rootEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (rootEntity is null)
            {
                throw new NotFoundException($"Could not find RootEntity '{request.Id}'");
            }

            rootEntity.Name = request.Name;
            rootEntity.EnumExample = request.EnumExample;
            rootEntity.NullableEnumExample = request.NullableEnumExample;
            rootEntity.Embedded = new EmbeddedObject(
                name: request.Embedded.Name,
                enumExample: request.Embedded.EnumExample,
                nullableEnumExample: request.Embedded.NullableEnumExample);

            _rootEntityRepository.Update(rootEntity);
        }
    }
}