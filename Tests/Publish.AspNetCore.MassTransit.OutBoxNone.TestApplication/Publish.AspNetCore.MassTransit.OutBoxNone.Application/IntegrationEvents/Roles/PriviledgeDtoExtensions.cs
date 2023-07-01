using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.DtoExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class PriviledgeDtoExtensions
    {
        public static PriviledgeDto MapToPriviledgeDto(this Priviledge projectFrom)
        {
            return new PriviledgeDto
            {
                Id = projectFrom.Id,
                RoleId = projectFrom.RoleId,
                Name = projectFrom.Name,
            };
        }
    }
}