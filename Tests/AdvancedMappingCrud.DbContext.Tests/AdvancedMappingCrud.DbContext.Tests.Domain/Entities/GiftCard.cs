using System;
using System.Collections.Generic;
using AdvancedMappingCrud.DbContext.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Domain.Entities
{
    public class GiftCard : IHasDomainEvent
    {
        public GiftCard()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public decimal Value { get; set; }

        public Guid? CustomerId { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}