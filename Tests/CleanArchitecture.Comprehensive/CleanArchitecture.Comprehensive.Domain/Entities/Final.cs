using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities
{
    public class Final : IHasDomainEvent
    {
        public Final()
        {
            Attribute = null!;
        }
        public Guid Id { get; set; }

        public int OneId { get; set; }

        public int TwoId { get; set; }

        public int ThreeId { get; set; }

        public int FourId { get; set; }

        public string Attribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}