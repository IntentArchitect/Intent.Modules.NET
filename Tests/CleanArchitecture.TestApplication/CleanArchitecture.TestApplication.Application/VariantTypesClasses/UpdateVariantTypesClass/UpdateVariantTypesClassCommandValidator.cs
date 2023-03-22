using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses.UpdateVariantTypesClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateVariantTypesClassCommandValidator : AbstractValidator<UpdateVariantTypesClassCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public UpdateVariantTypesClassCommandValidator()
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