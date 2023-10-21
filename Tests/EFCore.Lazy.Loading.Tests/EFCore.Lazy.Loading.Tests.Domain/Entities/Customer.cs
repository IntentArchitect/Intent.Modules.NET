using System;
using System.Collections.Generic;
using EFCore.Lazy.Loading.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace EFCore.Lazy.Loading.Tests.Domain.Entities
{
    public class Customer : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public Address? Address { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}