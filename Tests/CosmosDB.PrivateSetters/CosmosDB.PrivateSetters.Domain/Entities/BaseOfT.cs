using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public abstract class BaseOfT<T> : IHasDomainEvent
    {
        private string? _id;
        public BaseOfT()
        {
            Id = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            protected set => _id = value;
        }

        public T GenericAttribute { get; protected set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}