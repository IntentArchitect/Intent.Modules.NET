using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Orders.OrderConfirmed
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderConfirmedValidator : AbstractValidator<OrderConfirmed>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Merge)]
        public OrderConfirmedValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
            RuleFor(v => v.RefNo)
                .NotNull();

        }
    }
}