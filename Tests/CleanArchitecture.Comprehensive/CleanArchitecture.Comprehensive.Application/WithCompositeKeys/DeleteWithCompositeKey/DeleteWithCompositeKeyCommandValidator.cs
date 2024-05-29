using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.WithCompositeKeys.DeleteWithCompositeKey
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteWithCompositeKeyCommandValidator : AbstractValidator<DeleteWithCompositeKeyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteWithCompositeKeyCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}