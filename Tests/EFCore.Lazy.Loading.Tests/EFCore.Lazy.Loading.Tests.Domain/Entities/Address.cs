using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EFCore.Lazy.Loading.Tests.Domain.Entities
{
    public class Address
    {
        public Guid Id { get; set; }
    }
}