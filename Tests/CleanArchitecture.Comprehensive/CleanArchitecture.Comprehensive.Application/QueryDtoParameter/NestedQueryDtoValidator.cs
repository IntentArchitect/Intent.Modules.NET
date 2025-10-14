using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.QueryDtoParameter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NestedQueryDtoValidator : AbstractValidator<NestedQueryDto>
    {
        [IntentManaged(Mode.Merge)]
        public NestedQueryDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Numbers)
                .NotNull();
        }
    }
}