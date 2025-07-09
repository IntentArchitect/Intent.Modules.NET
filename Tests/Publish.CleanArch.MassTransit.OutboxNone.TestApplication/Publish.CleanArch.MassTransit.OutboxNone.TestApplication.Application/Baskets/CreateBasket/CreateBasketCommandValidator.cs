using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.CreateBasket
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBasketCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Number)
                .NotNull();
        }
    }
}