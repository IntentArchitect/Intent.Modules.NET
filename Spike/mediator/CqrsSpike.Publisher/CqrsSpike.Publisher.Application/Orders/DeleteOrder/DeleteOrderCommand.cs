using CqrsSpike.Publisher.Application.Common.Security;
using CqrsSpike.Publisher.Application.Security;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.CommandModels", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.Orders.DeleteOrder
{
    [Authorize(Roles = Permissions.RoleAdmin)]
    public class DeleteOrderCommand : ICommand
    {
        public DeleteOrderCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}