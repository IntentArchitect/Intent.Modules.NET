using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.CreateProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateProductCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Code)
                .NotNull()
                .MinimumLength(20)
                .MaximumLength(10);

            RuleFor(v => v.Name)
                .NotNull()
                .NotEmpty();
        }
    }
}