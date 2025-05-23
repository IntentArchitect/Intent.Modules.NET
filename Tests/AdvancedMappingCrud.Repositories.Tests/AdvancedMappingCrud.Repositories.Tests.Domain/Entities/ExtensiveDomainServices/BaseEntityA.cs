using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices
{
    public class BaseEntityA : IHasDomainEvent
    {
        public BaseEntityA()
        {
            BaseAttr = null!;
        }
        public Guid Id { get; set; }

        public string BaseAttr { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}