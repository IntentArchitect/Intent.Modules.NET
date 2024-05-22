using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Products.ChangeCategoriesProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeCategoriesProductCommandValidator : AbstractValidator<ChangeCategoriesProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeCategoriesProductCommandValidator()
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