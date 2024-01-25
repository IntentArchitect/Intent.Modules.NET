using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.ServiceCallHandlers.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Eventing.Messages
{
    public static class PersonDeletedEventExtensions
    {
        public static PersonDeletedEvent MapToPersonDeletedEvent(this Person projectFrom)
        {
            return new PersonDeletedEvent
            {
                Id = projectFrom.Id,
                Name = projectFrom.Name,
            };
        }
    }
}