using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.EntityListEnums.DeleteEntityListEnum
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteEntityListEnumCommandHandler : IRequestHandler<DeleteEntityListEnumCommand>
    {
        private readonly IEntityListEnumRepository _entityListEnumRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteEntityListEnumCommandHandler(IEntityListEnumRepository entityListEnumRepository)
        {
            _entityListEnumRepository = entityListEnumRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteEntityListEnumCommand request, CancellationToken cancellationToken)
        {
            var entityListEnum = await _entityListEnumRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entityListEnum is null)
            {
                throw new NotFoundException($"Could not find EntityListEnum '{request.Id}'");
            }

            _entityListEnumRepository.Remove(entityListEnum);
        }
    }
}