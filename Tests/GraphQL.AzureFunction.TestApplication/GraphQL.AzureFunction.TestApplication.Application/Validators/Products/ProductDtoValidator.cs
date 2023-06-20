using System;
using FluentValidation;
using GraphQL.AzureFunction.TestApplication.Application.Products;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Validators.Products
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public ProductDtoValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}