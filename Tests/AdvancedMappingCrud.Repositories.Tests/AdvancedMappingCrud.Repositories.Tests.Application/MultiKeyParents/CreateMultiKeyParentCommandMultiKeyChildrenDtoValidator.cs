using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateMultiKeyParentCommandMultiKeyChildrenDtoValidator : AbstractValidator<CreateMultiKeyParentCommandMultiKeyChildrenDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateMultiKeyParentCommandMultiKeyChildrenDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ChildName)
                .NotNull();
        }
    }
}