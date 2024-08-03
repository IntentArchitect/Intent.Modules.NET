using System;
using EntityFrameworkCore.Postgres.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.ValueObjects
{
    public class PersonWithAddressNormal
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public AddressNormal AddressNormal { get; set; }
    }
}