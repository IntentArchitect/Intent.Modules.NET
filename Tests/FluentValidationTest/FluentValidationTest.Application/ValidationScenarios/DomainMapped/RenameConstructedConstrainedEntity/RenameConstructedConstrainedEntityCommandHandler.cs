using FluentValidationTest.Domain.Common.Exceptions;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.ConstructorOperationConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.RenameConstructedConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RenameConstructedConstrainedEntityCommandHandler : IRequestHandler<RenameConstructedConstrainedEntityCommand>
    {
        private readonly IConstructedConstrainedEntityRepository _constructedConstrainedEntityRepository;
        [IntentManaged(Mode.Merge)]
        public RenameConstructedConstrainedEntityCommandHandler(IConstructedConstrainedEntityRepository constructedConstrainedEntityRepository)
        {
            _constructedConstrainedEntityRepository = constructedConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(RenameConstructedConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var constructedConstrainedEntity = await _constructedConstrainedEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (constructedConstrainedEntity is null)
            {
                throw new NotFoundException($"Could not find ConstructedConstrainedEntity '{request.Id}'");
            }

            constructedConstrainedEntity.Rename(request.NewTitle, request.NewCode);
        }
    }
}