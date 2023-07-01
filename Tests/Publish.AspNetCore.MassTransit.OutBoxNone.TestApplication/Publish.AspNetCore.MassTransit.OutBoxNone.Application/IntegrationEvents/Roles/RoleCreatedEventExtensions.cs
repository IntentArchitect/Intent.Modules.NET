using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class RoleCreatedEventExtensions
    {
        public static RoleCreatedEvent MapToRoleCreatedEvent(this Role projectFrom)
        {
            return new RoleCreatedEvent
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name,
                Priviledges = projectFrom.Priviledges.Select(PriviledgeDtoExtensions.MapToPriviledgeDto).ToList(),
            };
        }
    }
}