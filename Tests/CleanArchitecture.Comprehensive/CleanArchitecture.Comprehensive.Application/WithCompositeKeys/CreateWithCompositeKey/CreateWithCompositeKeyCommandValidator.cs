using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.WithCompositeKeys.CreateWithCompositeKey
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateWithCompositeKeyCommandValidator : AbstractValidator<CreateWithCompositeKeyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateWithCompositeKeyCommandValidator()
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