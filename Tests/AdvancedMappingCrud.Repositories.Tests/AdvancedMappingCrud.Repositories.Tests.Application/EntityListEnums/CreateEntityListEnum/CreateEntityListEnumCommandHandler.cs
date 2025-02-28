using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums.CreateEntityListEnum
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEntityListEnumCommandHandler : IRequestHandler<CreateEntityListEnumCommand, Guid>
    {
        private readonly IEntityListEnumRepository _entityListEnumRepository;

        [IntentManaged(Mode.Merge)]
        public CreateEntityListEnumCommandHandler(IEntityListEnumRepository entityListEnumRepository)
        {
            _entityListEnumRepository = entityListEnumRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateEntityListEnumCommand request, CancellationToken cancellationToken)
        {
            var entityListEnum = new EntityListEnum
            {
                Name = request.Name,
                OrderStatuses = request.OrderStatuses.ToList()
            };

            _entityListEnumRepository.Add(entityListEnum);
            await _entityListEnumRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return entityListEnum.Id;
        }
    }
}