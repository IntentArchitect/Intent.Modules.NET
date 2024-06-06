using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateSubmissionItemDtoValidator : AbstractValidator<CreateSubmissionItemDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateSubmissionItemDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Key)
                .NotNull();

            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}