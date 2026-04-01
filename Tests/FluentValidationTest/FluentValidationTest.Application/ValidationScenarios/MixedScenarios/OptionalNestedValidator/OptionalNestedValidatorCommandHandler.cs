using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.OptionalNestedValidator
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OptionalNestedValidatorCommandHandler : IRequestHandler<OptionalNestedValidatorCommand>
    {
        [IntentManaged(Mode.Merge)]
        public OptionalNestedValidatorCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(OptionalNestedValidatorCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (OptionalNestedValidatorCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}