using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Products.CreateProduct
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
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}