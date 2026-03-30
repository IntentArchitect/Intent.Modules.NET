using FluentValidationTest.Application.ValidationScenarios.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.GetNullabilityConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetNullabilityConstrainedEntityQueryHandler : IRequestHandler<GetNullabilityConstrainedEntityQuery, SimplePayloadDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetNullabilityConstrainedEntityQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<SimplePayloadDto> Handle(
            GetNullabilityConstrainedEntityQuery request,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetNullabilityConstrainedEntityQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}