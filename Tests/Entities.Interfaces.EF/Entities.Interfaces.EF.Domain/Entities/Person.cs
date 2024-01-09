using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.Interfaces.EF.Domain.Entities
{
    public class Person : IPerson
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}