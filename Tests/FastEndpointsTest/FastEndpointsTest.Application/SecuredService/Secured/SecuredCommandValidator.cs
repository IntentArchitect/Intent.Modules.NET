using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.SecuredService.Secured
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SecuredCommandValidator : AbstractValidator<SecuredCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SecuredCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}