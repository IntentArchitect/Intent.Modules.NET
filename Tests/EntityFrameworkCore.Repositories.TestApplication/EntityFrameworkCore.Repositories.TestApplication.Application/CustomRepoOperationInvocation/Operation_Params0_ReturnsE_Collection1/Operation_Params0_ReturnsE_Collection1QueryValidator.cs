using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CustomRepoOperationInvocation.Operation_Params0_ReturnsE_Collection1
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Operation_Params0_ReturnsE_Collection1QueryValidator : AbstractValidator<Operation_Params0_ReturnsE_Collection1Query>
    {
        [IntentManaged(Mode.Merge)]
        public Operation_Params0_ReturnsE_Collection1QueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}