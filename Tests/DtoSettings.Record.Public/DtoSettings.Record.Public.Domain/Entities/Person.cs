using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DtoSettings.Record.Public.Domain.Entities
{
    public abstract class Person
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}