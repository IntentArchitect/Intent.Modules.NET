using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.OperationInvocation.CustomRepoOperationParams3Async
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomRepoOperationParams3AsyncQueryValidator : AbstractValidator<CustomRepoOperationParams3AsyncQuery>
    {
        [IntentManaged(Mode.Merge)]
        public CustomRepoOperationParams3AsyncQueryValidator()
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