using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class WithGuidId : IHasDomainEvent
    {
        private Guid? _id;

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            private set => _id = value;
        }

        public string Field { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}