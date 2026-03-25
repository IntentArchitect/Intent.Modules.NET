using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PatchDocumentCommandPermissionsDtoValidator : AbstractValidator<PatchDocumentCommandPermissionsDto>
    {
        [IntentManaged(Mode.Merge)]
        public PatchDocumentCommandPermissionsDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

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