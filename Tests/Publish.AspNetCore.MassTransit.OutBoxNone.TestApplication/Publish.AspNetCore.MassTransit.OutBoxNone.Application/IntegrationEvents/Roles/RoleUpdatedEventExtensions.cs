using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class RoleUpdatedEventExtensions
    {
        public static RoleUpdatedEvent MapToRoleUpdatedEvent(this Role projectFrom)
        {
            return new RoleUpdatedEvent
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name,
                Priviledges = projectFrom.Priviledges.Select(PriviledgeDtoExtensions.MapToPriviledgeDto).ToList(),
            };
        }
    }
}