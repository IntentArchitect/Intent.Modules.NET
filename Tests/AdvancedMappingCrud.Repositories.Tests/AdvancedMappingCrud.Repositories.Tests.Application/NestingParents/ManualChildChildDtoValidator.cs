using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ManualChildChildDtoValidator : AbstractValidator<ManualChildChildDto>
    {
        [IntentManaged(Mode.Merge)]
        public ManualChildChildDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}