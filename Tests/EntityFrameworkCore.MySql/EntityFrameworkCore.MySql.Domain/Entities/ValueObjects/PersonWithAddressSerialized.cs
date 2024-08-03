using System;
using EntityFrameworkCore.MySql.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.ValueObjects
{
    public class PersonWithAddressSerialized
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public AddressSerialized AddressSerialized { get; set; }
    }
}