using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOneCommandFinalsDtoValidator : AbstractValidator<CreateOneCommandFinalsDto>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOneCommandFinalsDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Attribute)
                .NotNull();
        }
    }
}