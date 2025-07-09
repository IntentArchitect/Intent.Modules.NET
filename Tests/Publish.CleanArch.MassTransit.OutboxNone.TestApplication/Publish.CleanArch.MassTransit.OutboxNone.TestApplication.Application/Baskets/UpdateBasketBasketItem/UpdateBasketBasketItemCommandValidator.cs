using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.UpdateBasketBasketItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateBasketBasketItemCommandValidator : AbstractValidator<UpdateBasketBasketItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateBasketBasketItemCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Description)
                .NotNull();
        }
    }
}