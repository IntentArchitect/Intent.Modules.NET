using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TableStorage.Tests.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.PartitionKey)
                .NotNull();

            RuleFor(v => v.RowKey)
                .NotNull();

            RuleFor(v => v.OrderNo)
                .NotNull();

            RuleFor(v => v.Customer)
                .NotNull()
                .SetValidator(provider.GetValidator<CreateOrderCustomerDto>()!);

            RuleFor(v => v.OrderLines)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateOrderOrderLineDto>()!));
        }
    }
}