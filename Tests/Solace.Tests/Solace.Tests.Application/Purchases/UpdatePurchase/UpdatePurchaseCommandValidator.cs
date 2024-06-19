using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Solace.Tests.Application.Purchases.UpdatePurchase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdatePurchaseCommandValidator : AbstractValidator<UpdatePurchaseCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdatePurchaseCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}