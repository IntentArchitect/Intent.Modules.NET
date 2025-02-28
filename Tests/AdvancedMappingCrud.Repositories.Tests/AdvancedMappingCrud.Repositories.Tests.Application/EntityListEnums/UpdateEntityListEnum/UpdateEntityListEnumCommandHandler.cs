using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums.UpdateEntityListEnum
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEntityListEnumCommandHandler : IRequestHandler<UpdateEntityListEnumCommand>
    {
        private readonly IEntityListEnumRepository _entityListEnumRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateEntityListEnumCommandHandler(IEntityListEnumRepository entityListEnumRepository)
        {
            _entityListEnumRepository = entityListEnumRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateEntityListEnumCommand request, CancellationToken cancellationToken)
        {
            var entityListEnum = await _entityListEnumRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entityListEnum is null)
            {
                throw new NotFoundException($"Could not find EntityListEnum '{request.Id}'");
            }

            entityListEnum.Name = request.Name;
            entityListEnum.OrderStatuses = request.OrderStatuses.ToList();
        }
    }
}