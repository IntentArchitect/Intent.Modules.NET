using System;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface IWithGuidId : IHasDomainEvent
    {
        Guid Id { get; set; }

        string Field { get; set; }
    }
}