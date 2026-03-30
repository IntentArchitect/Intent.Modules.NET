using FluentValidationTest.Application.ValidationScenarios.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.GetTextConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetTextConstrainedEntityQueryHandler : IRequestHandler<GetTextConstrainedEntityQuery, SimplePayloadDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetTextConstrainedEntityQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<SimplePayloadDto> Handle(
            GetTextConstrainedEntityQuery request,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetTextConstrainedEntityQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}