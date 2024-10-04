using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities
{
    public abstract class BaseClass : IHasDomainEvent
    {
        public Guid Id { get; protected set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}