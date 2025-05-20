using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.SimpleProducts
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SimpleProductCreateDtoValidator : AbstractValidator<SimpleProductCreateDto>
    {
        [IntentManaged(Mode.Merge)]
        public SimpleProductCreateDtoValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();

            RuleFor(v => v.Value)
                .NotNull();
        }
    }
}