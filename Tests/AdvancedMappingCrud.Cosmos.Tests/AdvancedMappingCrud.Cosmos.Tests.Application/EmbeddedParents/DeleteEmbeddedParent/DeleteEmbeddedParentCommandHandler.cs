using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.DeleteEmbeddedParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteEmbeddedParentCommandHandler : IRequestHandler<DeleteEmbeddedParentCommand>
    {
        private readonly IEmbeddedParentRepository _embeddedParentRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteEmbeddedParentCommandHandler(IEmbeddedParentRepository embeddedParentRepository)
        {
            _embeddedParentRepository = embeddedParentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteEmbeddedParentCommand request, CancellationToken cancellationToken)
        {
            var embeddedParent = await _embeddedParentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (embeddedParent is null)
            {
                throw new NotFoundException($"Could not find EmbeddedParent '{request.Id}'");
            }

            _embeddedParentRepository.Remove(embeddedParent);
        }
    }
}