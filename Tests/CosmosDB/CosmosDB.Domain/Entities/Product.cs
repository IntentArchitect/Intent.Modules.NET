using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        private string? _id;
        public Product()
        {
            Id = null!;
            Name = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public IList<string> CategoriesIds { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}