using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public class PriviledgeDto
    {
        public PriviledgeDto()
        {
        }

        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string Name { get; set; }

        public static PriviledgeDto Create(Guid id, Guid roleId, string name)
        {
            return new PriviledgeDto
            {
                Id = id,
                RoleId = roleId,
                Name = name
            };
        }
    }
}