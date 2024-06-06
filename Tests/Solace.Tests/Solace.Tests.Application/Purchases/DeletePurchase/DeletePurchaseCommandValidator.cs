using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Solace.Tests.Application.Purchases.DeletePurchase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeletePurchaseCommandValidator : AbstractValidator<DeletePurchaseCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeletePurchaseCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}