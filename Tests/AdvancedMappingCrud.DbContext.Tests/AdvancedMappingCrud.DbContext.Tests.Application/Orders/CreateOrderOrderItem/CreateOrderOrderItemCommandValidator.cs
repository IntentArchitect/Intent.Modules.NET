using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Orders.CreateOrderOrderItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderOrderItemCommandValidator : AbstractValidator<CreateOrderOrderItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderOrderItemCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}