using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateNestingParentCommandNestingChildrenDtoValidator : AbstractValidator<CreateNestingParentCommandNestingChildrenDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateNestingParentCommandNestingChildrenDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Description)
                .NotNull();

            RuleFor(v => v.ChildChild)
                .NotNull()
                .SetValidator(provider.GetValidator<ManualChildChildDto>()!);
        }
    }
}