using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TableStorage.Tests.Application.Orders.UpdateOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateOrderCommandValidator(IValidatorProvider provider)
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
                .SetValidator(provider.GetValidator<UpdateOrderCustomerDto>()!);
        }
    }
}