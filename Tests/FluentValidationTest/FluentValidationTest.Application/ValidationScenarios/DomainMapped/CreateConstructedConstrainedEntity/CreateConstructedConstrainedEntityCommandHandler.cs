using FluentValidationTest.Domain.Entities.ValidationScenarios.ConstructorOperationConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.ConstructorOperationConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateConstructedConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateConstructedConstrainedEntityCommandHandler : IRequestHandler<CreateConstructedConstrainedEntityCommand>
    {
        private readonly IConstructedConstrainedEntityRepository _constructedConstrainedEntityRepository;
        [IntentManaged(Mode.Merge)]
        public CreateConstructedConstrainedEntityCommandHandler(IConstructedConstrainedEntityRepository constructedConstrainedEntityRepository)
        {
            _constructedConstrainedEntityRepository = constructedConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateConstructedConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var constructedConstrainedEntity = new ConstructedConstrainedEntity(
                title: request.Title,
                code: request.Code)
            {
                OptionalComment = request.OptionalComment
            };

            _constructedConstrainedEntityRepository.Add(constructedConstrainedEntity);
        }
    }
}