using System;
using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public abstract class BaseOfT<T> : IBaseOfT<T>, IHasDomainEvent
    {
        private string? _id;
        public BaseOfT()
        {
            Id = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public T GenericAttribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}