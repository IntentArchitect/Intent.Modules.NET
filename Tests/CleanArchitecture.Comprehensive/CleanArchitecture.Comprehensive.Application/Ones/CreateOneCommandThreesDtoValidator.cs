using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOneCommandThreesDtoValidator : AbstractValidator<CreateOneCommandThreesDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOneCommandThreesDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Finals)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateOneCommandFinalsDto>()!));
        }
    }
}