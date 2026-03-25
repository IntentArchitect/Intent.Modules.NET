using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDocumentCommandPermissionsDtoValidator : AbstractValidator<CreateDocumentCommandPermissionsDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDocumentCommandPermissionsDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Role)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.PrincipalId)
                .NotNull();

            RuleFor(v => v.PrincipalType)
                .NotNull();
        }
    }
}