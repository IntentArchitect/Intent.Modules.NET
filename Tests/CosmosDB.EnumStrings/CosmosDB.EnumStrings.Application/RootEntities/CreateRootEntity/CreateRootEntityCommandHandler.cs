using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EnumStrings.Domain;
using CosmosDB.EnumStrings.Domain.Entities;
using CosmosDB.EnumStrings.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.CreateRootEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateRootEntityCommandHandler : IRequestHandler<CreateRootEntityCommand, string>
    {
        private readonly IRootEntityRepository _rootEntityRepository;

        [IntentManaged(Mode.Merge)]
        public CreateRootEntityCommandHandler(IRootEntityRepository rootEntityRepository)
        {
            _rootEntityRepository = rootEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateRootEntityCommand request, CancellationToken cancellationToken)
        {
            var rootEntity = new RootEntity
            {
                Name = request.Name,
                EnumExample = request.EnumExample,
                NullableEnumExample = request.NullableEnumExample,
                Embedded = new EmbeddedObject(
                    name: request.Embedded.Name,
                    enumExample: request.Embedded.EnumExample,
                    nullableEnumExample: request.Embedded.NullableEnumExample),
                NestedEntities = request.NestedEntities
                    .Select(ne => new NestedEntity
                    {
                        Name = ne.Name,
                        EnumExample = ne.EnumExample,
                        NullableEnumExample = ne.NullableEnumExample
                    })
                    .ToList()
            };

            _rootEntityRepository.Add(rootEntity);
            await _rootEntityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return rootEntity.Id;
        }
    }
}