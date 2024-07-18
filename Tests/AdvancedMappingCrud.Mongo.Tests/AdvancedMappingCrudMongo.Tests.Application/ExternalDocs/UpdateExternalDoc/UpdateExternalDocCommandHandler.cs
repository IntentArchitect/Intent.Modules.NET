using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.UpdateExternalDoc
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateExternalDocCommandHandler : IRequestHandler<UpdateExternalDocCommand>
    {
        private readonly IExternalDocRepository _externalDocRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateExternalDocCommandHandler(IExternalDocRepository externalDocRepository)
        {
            _externalDocRepository = externalDocRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateExternalDocCommand request, CancellationToken cancellationToken)
        {
            var externalDoc = await _externalDocRepository.FindByIdAsync(request.Id, cancellationToken);
            if (externalDoc is null)
            {
                throw new NotFoundException($"Could not find ExternalDoc '{request.Id}'");
            }

            externalDoc.Name = request.Name;
            externalDoc.Thing = request.Thing;

            _externalDocRepository.Update(externalDoc);
        }
    }
}