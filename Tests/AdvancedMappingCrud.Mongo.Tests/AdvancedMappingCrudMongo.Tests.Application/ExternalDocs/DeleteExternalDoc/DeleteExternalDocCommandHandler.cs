using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.DeleteExternalDoc
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteExternalDocCommandHandler : IRequestHandler<DeleteExternalDocCommand>
    {
        private readonly IExternalDocRepository _externalDocRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteExternalDocCommandHandler(IExternalDocRepository externalDocRepository)
        {
            _externalDocRepository = externalDocRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteExternalDocCommand request, CancellationToken cancellationToken)
        {
            var externalDoc = await _externalDocRepository.FindByIdAsync(request.Id, cancellationToken);
            if (externalDoc is null)
            {
                throw new NotFoundException($"Could not find ExternalDoc '{request.Id}'");
            }

            _externalDocRepository.Remove(externalDoc);
        }
    }
}