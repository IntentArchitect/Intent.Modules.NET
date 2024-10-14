using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Orders.UpdateOrderWithDomainService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateOrderWithDomainServiceCommandValidator : AbstractValidator<UpdateOrderWithDomainServiceCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateOrderWithDomainServiceCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.OrderItemDetails)
                .NotNull();
        }
    }
}