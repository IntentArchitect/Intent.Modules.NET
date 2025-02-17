using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface ICountry
    {
        Guid Id { get; set; }

        string Name { get; set; }
    }
}