using System;
using EntityFrameworkCore.SqlServer.EF10.Domain.ValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities.ValueObjects
{
    public class PersonWithAddressNormal
    {
        public PersonWithAddressNormal()
        {
            Name = null!;
            AddressNormal = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public AddressNormal AddressNormal { get; set; }
    }
}