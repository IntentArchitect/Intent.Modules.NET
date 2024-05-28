using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TrainingModel.Tests.Domain.Entities
{
    public class Category : IHasDomainEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}