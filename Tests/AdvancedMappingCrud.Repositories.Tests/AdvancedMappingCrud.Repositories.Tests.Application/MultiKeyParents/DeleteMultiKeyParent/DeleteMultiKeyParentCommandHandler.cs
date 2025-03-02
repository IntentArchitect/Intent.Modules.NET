using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents.DeleteMultiKeyParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteMultiKeyParentCommandHandler : IRequestHandler<DeleteMultiKeyParentCommand>
    {
        private readonly IMultiKeyParentRepository _multiKeyParentRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteMultiKeyParentCommandHandler(IMultiKeyParentRepository multiKeyParentRepository)
        {
            _multiKeyParentRepository = multiKeyParentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteMultiKeyParentCommand request, CancellationToken cancellationToken)
        {
            var multiKeyParent = await _multiKeyParentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (multiKeyParent is null)
            {
                throw new NotFoundException($"Could not find MultiKeyParent '{request.Id}'");
            }

            _multiKeyParentRepository.Remove(multiKeyParent);
        }
    }
}