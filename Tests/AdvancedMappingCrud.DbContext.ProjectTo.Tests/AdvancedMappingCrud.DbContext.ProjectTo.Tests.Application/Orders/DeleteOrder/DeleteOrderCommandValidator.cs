using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Orders.DeleteOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteOrderCommandValidator()
        {
        }
    }
}