using System;
using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Domain.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.BasicMappingMapToValueObjects
{
    public class Submission : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string SubmissionType { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}