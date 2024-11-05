using System;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface IDepartment : IHasDomainEvent
    {
        Guid Id { get; set; }

        Guid? UniversityId { get; set; }

        string Name { get; set; }
    }
}