using FluentValidationTest.Domain.Common.Exceptions;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateNullabilityConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateNullabilityConstrainedEntityCommandHandler : IRequestHandler<UpdateNullabilityConstrainedEntityCommand>
    {
        private readonly INullabilityConstrainedEntityRepository _nullabilityConstrainedEntityRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateNullabilityConstrainedEntityCommandHandler(INullabilityConstrainedEntityRepository nullabilityConstrainedEntityRepository)
        {
            _nullabilityConstrainedEntityRepository = nullabilityConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateNullabilityConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var nullabilityConstrainedEntity = await _nullabilityConstrainedEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (nullabilityConstrainedEntity is null)
            {
                throw new NotFoundException($"Could not find NullabilityConstrainedEntity '{request.Id}'");
            }

            nullabilityConstrainedEntity.RequiredString = request.RequiredString;
            nullabilityConstrainedEntity.OptionalString = request.OptionalString;
            nullabilityConstrainedEntity.RequiredInt = request.RequiredInt;
            nullabilityConstrainedEntity.OptionalInt = request.OptionalInt;
            nullabilityConstrainedEntity.RequiredGuidValue = request.RequiredGuidValue;
            nullabilityConstrainedEntity.OptionalGuidValue = request.OptionalGuidValue;
            nullabilityConstrainedEntity.RequiredDateValue = request.RequiredDateValue;
            nullabilityConstrainedEntity.OptionalDateValue = request.OptionalDateValue;
        }
    }
}