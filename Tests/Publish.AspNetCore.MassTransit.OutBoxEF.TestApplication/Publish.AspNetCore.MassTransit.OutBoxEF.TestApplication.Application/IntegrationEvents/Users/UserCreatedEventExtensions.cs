using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class UserCreatedEventExtensions
    {
        public static UserCreatedEvent MapToUserCreatedEvent(this User projectFrom)
        {
            return new UserCreatedEvent
            {
                Id = projectFrom.Id,
                Email = projectFrom.Email,
                UserName = projectFrom.UserName,
                Preferences = projectFrom.Preferences.Select(PreferenceDtoExtensions.MapToPreferenceDto).ToList(),
            };
        }
    }
}