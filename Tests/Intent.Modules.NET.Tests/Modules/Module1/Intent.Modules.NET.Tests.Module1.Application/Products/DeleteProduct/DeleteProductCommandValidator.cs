using FluentValidation;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Products.DeleteProduct;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Products.DeleteProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteProductCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}