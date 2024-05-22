using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Products.ChangeCategoriesSyncProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ChangeCategoriesSyncProductCommandValidator : AbstractValidator<ChangeCategoriesSyncProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ChangeCategoriesSyncProductCommandValidator()
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