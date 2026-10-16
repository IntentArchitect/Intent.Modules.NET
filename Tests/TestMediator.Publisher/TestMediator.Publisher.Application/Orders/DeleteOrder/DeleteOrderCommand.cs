using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Publisher.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace TestMediator.Publisher.Application.Orders.DeleteOrder
{
    [Authorize]
    public class DeleteOrderCommand : ICommand
    {
        public DeleteOrderCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}