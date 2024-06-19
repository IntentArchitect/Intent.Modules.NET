using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Solace.Tests.Application.Purchases.CreatePurchase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreatePurchaseCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}