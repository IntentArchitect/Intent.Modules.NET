using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Domain.Entities
{
    public class GetAllImplementationCustomer : IHasDomainEvent
    {
        private string? _id;

        public GetAllImplementationCustomer()
        {
            Id = null!;
            Name = null!;
            GetAllImplementationOrder = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public GetAllImplementationOrder GetAllImplementationOrder { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}