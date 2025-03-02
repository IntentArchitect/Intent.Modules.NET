using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateMultiKeyParentCommandMultiKeyChildrenDtoValidator : AbstractValidator<UpdateMultiKeyParentCommandMultiKeyChildrenDto>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateMultiKeyParentCommandMultiKeyChildrenDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.ChildName)
                .NotNull();

            RuleFor(v => v.RefNo)
                .NotNull();
        }
    }
}