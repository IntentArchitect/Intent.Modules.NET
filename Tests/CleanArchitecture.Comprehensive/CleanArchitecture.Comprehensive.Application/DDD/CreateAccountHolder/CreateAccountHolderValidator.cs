using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.DDD.CreateAccountHolder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateAccountHolderValidator : AbstractValidator<CreateAccountHolder>
    {
        [IntentManaged(Mode.Merge)]
        public CreateAccountHolderValidator()
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