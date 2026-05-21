using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities
{
    public class GiftCard : IHasDomainEvent
    {
        public GiftCard()
        {
            CardCode = null!;
        }

        public string CardCode { get; set; }

        public decimal Value { get; set; }

        public Guid? UserId { get; set; }

        public virtual User? User { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}