using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.CreateEmbeddedParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEmbeddedParentCommandHandler : IRequestHandler<CreateEmbeddedParentCommand, string>
    {
        private readonly IEmbeddedParentRepository _embeddedParentRepository;

        [IntentManaged(Mode.Merge)]
        public CreateEmbeddedParentCommandHandler(IEmbeddedParentRepository embeddedParentRepository)
        {
            _embeddedParentRepository = embeddedParentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateEmbeddedParentCommand request, CancellationToken cancellationToken)
        {
            var embeddedParent = new EmbeddedParent
            {
                Name = request.Name,
                Children = request.Children
                    .Select(c => new EmbeddedChild(
                        name: c.Name,
                        age: c.Age))
                    .ToList()
            };

            _embeddedParentRepository.Add(embeddedParent);
            await _embeddedParentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return embeddedParent.Id;
        }
    }
}