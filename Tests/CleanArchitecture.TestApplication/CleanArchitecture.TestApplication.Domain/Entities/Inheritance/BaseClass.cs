using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.Inheritance
{
    public class BaseClass : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string BaseAttr { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}