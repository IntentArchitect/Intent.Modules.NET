using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.SecuredService.SecuredServiceWithAndRoles
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SecuredServiceWithAndRolesValidator : AbstractValidator<SecuredServiceWithAndRoles>
    {
        [IntentManaged(Mode.Merge)]
        public SecuredServiceWithAndRolesValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}