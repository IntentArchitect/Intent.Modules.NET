using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace CosmosDB.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public ICollection<string> CategoriesIds { get; set; } = new List<string>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}