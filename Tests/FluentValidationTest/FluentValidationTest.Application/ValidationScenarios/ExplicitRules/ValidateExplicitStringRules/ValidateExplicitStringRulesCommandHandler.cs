using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.ExplicitRules.ValidateExplicitStringRules
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ValidateExplicitStringRulesCommandHandler : IRequestHandler<ValidateExplicitStringRulesCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateExplicitStringRulesCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ValidateExplicitStringRulesCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ValidateExplicitStringRulesCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}