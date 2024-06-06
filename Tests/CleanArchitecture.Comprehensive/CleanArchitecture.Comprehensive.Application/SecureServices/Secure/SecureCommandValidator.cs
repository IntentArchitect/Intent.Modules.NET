using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.SecureServices.Secure
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SecureCommandValidator : AbstractValidator<SecureCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SecureCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Message)
                .NotNull();
        }
    }
}