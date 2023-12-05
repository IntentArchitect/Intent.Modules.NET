using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.SecuredService.SecuredServiceWithAndRoles
{
    public class SecuredServiceWithAndRolesValidator : AbstractValidator<SecuredServiceWithAndRoles>
    {
        [IntentManaged(Mode.Merge)]
        public SecuredServiceWithAndRolesValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}