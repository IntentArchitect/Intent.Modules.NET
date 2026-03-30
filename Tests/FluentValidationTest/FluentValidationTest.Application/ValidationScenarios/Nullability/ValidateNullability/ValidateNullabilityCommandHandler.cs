using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Nullability.ValidateNullability
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ValidateNullabilityCommandHandler : IRequestHandler<ValidateNullabilityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateNullabilityCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ValidateNullabilityCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ValidateNullabilityCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}