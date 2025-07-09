using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.WithCompositeKeys.UpdateWithCompositeKey
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateWithCompositeKeyCommandValidator : AbstractValidator<UpdateWithCompositeKeyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateWithCompositeKeyCommandValidator()
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