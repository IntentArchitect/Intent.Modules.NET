using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Brands.UpdateBrand
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBrandCommandValidator()
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