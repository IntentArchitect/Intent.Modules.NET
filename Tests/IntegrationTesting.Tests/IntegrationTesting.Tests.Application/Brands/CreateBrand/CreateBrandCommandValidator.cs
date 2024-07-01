using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Brands.CreateBrand
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBrandCommandValidator()
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