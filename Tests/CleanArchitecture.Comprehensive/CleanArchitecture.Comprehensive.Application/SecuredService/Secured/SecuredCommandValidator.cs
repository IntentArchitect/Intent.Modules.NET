using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.SecuredService.Secured
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SecuredCommandValidator : AbstractValidator<SecuredCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SecuredCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}