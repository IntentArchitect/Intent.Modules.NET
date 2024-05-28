using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Products.CaptureCategoriesProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CaptureCategoriesProductCommandValidator : AbstractValidator<CaptureCategoriesProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CaptureCategoriesProductCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.CategoryNames)
                .NotNull();
        }
    }
}