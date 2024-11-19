using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Orders.DeleteOrderOrderItem
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteOrderOrderItemCommandValidator : AbstractValidator<DeleteOrderOrderItemCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteOrderOrderItemCommandValidator()
        {
        }
    }
}