using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.AdvChanceAccountHolderName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AdvChanceAccountHolderNameValidator : AbstractValidator<AdvChanceAccountHolderName>
    {
        [IntentManaged(Mode.Merge)]
        public AdvChanceAccountHolderNameValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}