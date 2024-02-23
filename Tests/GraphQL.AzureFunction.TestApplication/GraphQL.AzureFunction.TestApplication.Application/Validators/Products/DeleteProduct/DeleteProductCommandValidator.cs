using System;
using FluentValidation;
using GraphQL.AzureFunction.TestApplication.Application.Products.DeleteProduct;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace GraphQL.AzureFunction.TestApplication.Application.Validators.Products.DeleteProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public DeleteProductCommandValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}