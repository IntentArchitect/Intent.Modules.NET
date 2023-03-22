using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses.CreateVariantTypesClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateVariantTypesClassCommandValidator : AbstractValidator<CreateVariantTypesClassCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public CreateVariantTypesClassCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.StrCollection)
                .NotNull();

            RuleFor(v => v.IntCollection)
                .NotNull();

        }
    }
}