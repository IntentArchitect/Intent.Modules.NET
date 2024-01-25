using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Eventing.Messages
{
    public static class PersonUpdatedEventExtensions
    {
        public static PersonUpdatedEvent MapToPersonUpdatedEvent(this Person projectFrom)
        {
            return new PersonUpdatedEvent
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name,
            };
        }
    }
}