using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.ExplicitRules.ValidateCrossFieldRules
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ValidateCrossFieldRulesCommandHandler : IRequestHandler<ValidateCrossFieldRulesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateCrossFieldRulesCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ValidateCrossFieldRulesCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ValidateCrossFieldRulesCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}