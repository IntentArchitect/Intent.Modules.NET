using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class RoleDeletedEventExtensions
    {
        public static RoleDeletedEvent MapToRoleDeletedEvent(this Role projectFrom)
        {
            return new RoleDeletedEvent
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name,
            };
        }
    }
}