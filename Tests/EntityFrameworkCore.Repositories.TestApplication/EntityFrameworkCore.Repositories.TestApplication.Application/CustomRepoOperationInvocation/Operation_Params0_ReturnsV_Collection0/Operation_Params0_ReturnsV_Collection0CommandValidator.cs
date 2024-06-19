using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CustomRepoOperationInvocation.Operation_Params0_ReturnsV_Collection0
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Operation_Params0_ReturnsV_Collection0CommandValidator : AbstractValidator<Operation_Params0_ReturnsV_Collection0Command>
    {
        [IntentManaged(Mode.Merge)]
        public Operation_Params0_ReturnsV_Collection0CommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}