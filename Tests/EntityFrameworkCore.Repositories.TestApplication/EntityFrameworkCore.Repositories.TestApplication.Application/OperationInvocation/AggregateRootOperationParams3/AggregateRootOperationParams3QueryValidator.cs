using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.OperationInvocation.AggregateRootOperationParams3
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AggregateRootOperationParams3QueryValidator : AbstractValidator<AggregateRootOperationParams3Query>
    {
        [IntentManaged(Mode.Merge)]
        public AggregateRootOperationParams3QueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.AttributeBinary)
                .NotNull();

            RuleFor(v => v.AttributeString)
                .NotNull();

            RuleFor(v => v.Tag)
                .NotNull()
                .MaximumLength(125);

            RuleFor(v => v.StrParam)
                .NotNull();
        }
    }
}