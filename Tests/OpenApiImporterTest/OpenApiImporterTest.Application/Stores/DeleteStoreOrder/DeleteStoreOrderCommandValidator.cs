using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.DeleteStoreOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteStoreOrderCommandValidator : AbstractValidator<DeleteStoreOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteStoreOrderCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}