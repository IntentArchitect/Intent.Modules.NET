using FluentValidationTest.Domain.Entities.ValidationScenarios.Nullability;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateNullabilityConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateNullabilityConstrainedEntityCommandHandler : IRequestHandler<CreateNullabilityConstrainedEntityCommand>
    {
        private readonly INullabilityConstrainedEntityRepository _nullabilityConstrainedEntityRepository;
        [IntentManaged(Mode.Merge)]
        public CreateNullabilityConstrainedEntityCommandHandler(INullabilityConstrainedEntityRepository nullabilityConstrainedEntityRepository)
        {
            _nullabilityConstrainedEntityRepository = nullabilityConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateNullabilityConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var nullabilityConstrainedEntity = new NullabilityConstrainedEntity
            {
                RequiredString = request.RequiredString,
                OptionalString = request.OptionalString,
                RequiredInt = request.RequiredInt,
                OptionalInt = request.OptionalInt,
                RequiredGuidValue = request.RequiredGuidValue,
                OptionalGuidValue = request.OptionalGuidValue,
                RequiredDateValue = request.RequiredDateValue,
                OptionalDateValue = request.OptionalDateValue
            };

            _nullabilityConstrainedEntityRepository.Add(nullabilityConstrainedEntity);
        }
    }
}