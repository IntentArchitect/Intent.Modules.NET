using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.RichProducts.DeleteRichProduct
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteRichProductCommandValidator : AbstractValidator<DeleteRichProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteRichProductCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}