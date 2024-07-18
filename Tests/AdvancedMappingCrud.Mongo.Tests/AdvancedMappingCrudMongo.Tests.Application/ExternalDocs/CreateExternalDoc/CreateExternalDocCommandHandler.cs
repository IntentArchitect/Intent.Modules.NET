using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.CreateExternalDoc
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateExternalDocCommandHandler : IRequestHandler<CreateExternalDocCommand, long>
    {
        private readonly IExternalDocRepository _externalDocRepository;

        [IntentManaged(Mode.Merge)]
        public CreateExternalDocCommandHandler(IExternalDocRepository externalDocRepository)
        {
            _externalDocRepository = externalDocRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<long> Handle(CreateExternalDocCommand request, CancellationToken cancellationToken)
        {
            var externalDoc = new ExternalDoc
            {
                Id = request.Id,
                Name = request.Name,
                Thing = request.Thing
            };

            _externalDocRepository.Add(externalDoc);
            await _externalDocRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return externalDoc.Id;
        }
    }
}