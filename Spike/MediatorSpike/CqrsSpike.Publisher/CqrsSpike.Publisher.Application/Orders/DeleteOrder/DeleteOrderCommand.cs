using CqrsSpike.Publisher.Application.Common.Security;
using CqrsSpike.Publisher.Application.Security;
using Mediator;

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