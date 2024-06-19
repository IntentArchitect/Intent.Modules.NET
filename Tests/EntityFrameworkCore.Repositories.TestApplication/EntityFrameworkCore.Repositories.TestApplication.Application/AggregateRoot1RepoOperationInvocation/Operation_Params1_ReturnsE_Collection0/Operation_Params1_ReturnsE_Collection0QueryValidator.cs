using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.AggregateRoot1RepoOperationInvocation.Operation_Params1_ReturnsE_Collection0
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class Operation_Params1_ReturnsE_Collection0QueryValidator : AbstractValidator<Operation_Params1_ReturnsE_Collection0Query>
    {
        [IntentManaged(Mode.Merge)]
        public Operation_Params1_ReturnsE_Collection0QueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.AttributeBinary)
                .NotNull();

            RuleFor(v => v.AttributeString)
                .NotNull();
        }
    }
}