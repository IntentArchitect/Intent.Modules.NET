using FluentValidationTest.Application.ValidationScenarios.Nullability;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.GetNumericConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetNumericConstrainedEntityQueryHandler : IRequestHandler<GetNumericConstrainedEntityQuery, SimplePayloadDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetNumericConstrainedEntityQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<SimplePayloadDto> Handle(
            GetNumericConstrainedEntityQuery request,
            CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetNumericConstrainedEntityQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}