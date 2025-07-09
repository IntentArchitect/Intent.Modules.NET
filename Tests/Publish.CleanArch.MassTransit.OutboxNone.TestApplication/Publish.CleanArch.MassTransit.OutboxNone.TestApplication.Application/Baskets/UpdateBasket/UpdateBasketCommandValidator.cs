using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.UpdateBasket
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBasketCommandValidator : AbstractValidator<UpdateBasketCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBasketCommandValidator()
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