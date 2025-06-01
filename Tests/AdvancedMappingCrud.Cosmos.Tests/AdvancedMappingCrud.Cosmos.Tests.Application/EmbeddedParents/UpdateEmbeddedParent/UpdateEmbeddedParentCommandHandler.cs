using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.UpdateEmbeddedParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEmbeddedParentCommandHandler : IRequestHandler<UpdateEmbeddedParentCommand>
    {
        private readonly IEmbeddedParentRepository _embeddedParentRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateEmbeddedParentCommandHandler(IEmbeddedParentRepository embeddedParentRepository)
        {
            _embeddedParentRepository = embeddedParentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateEmbeddedParentCommand request, CancellationToken cancellationToken)
        {
            var embeddedParent = await _embeddedParentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (embeddedParent is null)
            {
                throw new NotFoundException($"Could not find EmbeddedParent '{request.Id}'");
            }

            embeddedParent.Name = request.Name;
            embeddedParent.Children = UpdateHelper.CreateOrUpdateCollection(embeddedParent.Children, request.Children, (e, d) => e.Equals(new EmbeddedChild(
                name: d.Name,
                age: d.Age)), CreateOrUpdateEmbeddedChild);

            _embeddedParentRepository.Update(embeddedParent);
        }

        [IntentManaged(Mode.Fully)]
        private static EmbeddedChild CreateOrUpdateEmbeddedChild(
            EmbeddedChild? valueObject,
            UpdateEmbeddedParentEmbeddedChildDto dto)
        {
            if (valueObject is null)
            {
                return new EmbeddedChild(
                    name: dto.Name,
                    age: dto.Age);
            }
            return valueObject;
        }
    }
}