using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class UserDeletedEventExtensions
    {
        public static UserDeletedEvent MapToUserDeletedEvent(this User projectFrom)
        {
            return new UserDeletedEvent
            {
                Id = projectFrom.Id,
                Email = projectFrom.Email,
                UserName = projectFrom.UserName,
                Type = projectFrom.Type,
            };
        }
    }
}