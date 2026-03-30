using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.ExplicitRules.ValidateExplicitNumericRules
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ValidateExplicitNumericRulesCommandHandler : IRequestHandler<ValidateExplicitNumericRulesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateExplicitNumericRulesCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ValidateExplicitNumericRulesCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ValidateExplicitNumericRulesCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}