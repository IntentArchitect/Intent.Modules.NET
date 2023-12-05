using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class LineItem
    {
        private List<string> _tags = new List<string>();
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            private set => _id = value;
        }

        public string Description { get; private set; }

        public int Quantity { get; private set; }

        public string ProductId { get; private set; }

        public IReadOnlyCollection<string> Tags
        {
            get => _tags.AsReadOnly();
            private set => _tags = new List<string>(value);
        }
    }
}