using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOneCommandTwosDtoValidator : AbstractValidator<CreateOneCommandTwosDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOneCommandTwosDtoValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.Threes)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateOneCommandThreesDto>()!));
        }
    }
}