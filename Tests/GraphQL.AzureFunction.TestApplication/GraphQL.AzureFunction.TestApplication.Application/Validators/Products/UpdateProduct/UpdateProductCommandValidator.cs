using System;
using FluentValidation;
using GraphQL.AzureFunction.TestApplication.Application.Products.UpdateProduct;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Validators.Products.UpdateProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateProductCommandValidator()
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