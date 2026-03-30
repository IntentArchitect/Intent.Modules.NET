using FluentValidationTest.Application.ValidationScenarios.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.GetPatternConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetPatternConstrainedEntityQueryHandler : IRequestHandler<GetPatternConstrainedEntityQuery, SimplePayloadDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetPatternConstrainedEntityQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<SimplePayloadDto> Handle(
            GetPatternConstrainedEntityQuery request,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetPatternConstrainedEntityQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}