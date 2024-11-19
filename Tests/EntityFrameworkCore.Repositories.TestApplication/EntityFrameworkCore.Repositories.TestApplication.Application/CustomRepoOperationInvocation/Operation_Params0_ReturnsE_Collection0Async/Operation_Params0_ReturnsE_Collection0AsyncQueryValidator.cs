using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CustomRepoOperationInvocation.Operation_Params0_ReturnsE_Collection0Async
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Operation_Params0_ReturnsE_Collection0AsyncQueryValidator : AbstractValidator<Operation_Params0_ReturnsE_Collection0AsyncQuery>
    {
        [IntentManaged(Mode.Merge)]
        public Operation_Params0_ReturnsE_Collection0AsyncQueryValidator()
        {
        }
    }
}