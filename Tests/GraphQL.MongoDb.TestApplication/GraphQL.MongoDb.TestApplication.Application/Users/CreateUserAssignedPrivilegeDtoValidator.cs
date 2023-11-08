using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users
{
    public class CreateUserAssignedPrivilegeDtoValidator : AbstractValidator<CreateUserAssignedPrivilegeDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserAssignedPrivilegeDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PrivilegeId)
                .NotNull();
        }
    }
}