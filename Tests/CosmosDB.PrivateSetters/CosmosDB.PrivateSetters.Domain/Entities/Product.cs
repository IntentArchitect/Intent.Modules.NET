using System;
using System.Collections.Generic;
using CosmosDB.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        private List<string> _categoriesIds = new List<string>();
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public string Name { get; private set; }

        public IReadOnlyCollection<string> CategoriesIds
        {
            get => _categoriesIds.AsReadOnly();
            private set => _categoriesIds = new List<string>(value);
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}