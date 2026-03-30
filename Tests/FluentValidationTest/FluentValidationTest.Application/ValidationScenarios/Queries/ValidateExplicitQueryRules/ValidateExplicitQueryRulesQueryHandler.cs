using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Queries.ValidateExplicitQueryRules
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ValidateExplicitQueryRulesQueryHandler : IRequestHandler<ValidateExplicitQueryRulesQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateExplicitQueryRulesQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(ValidateExplicitQueryRulesQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ValidateExplicitQueryRulesQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}