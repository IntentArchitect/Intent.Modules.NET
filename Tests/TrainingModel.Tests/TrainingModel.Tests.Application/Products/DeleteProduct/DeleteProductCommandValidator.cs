using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Products.DeleteProduct
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
        }
    }
}