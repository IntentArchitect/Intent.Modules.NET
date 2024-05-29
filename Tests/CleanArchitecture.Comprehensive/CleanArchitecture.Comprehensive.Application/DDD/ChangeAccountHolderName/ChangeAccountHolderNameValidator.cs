using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.ChangeAccountHolderName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeAccountHolderNameValidator : AbstractValidator<ChangeAccountHolderName>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeAccountHolderNameValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}