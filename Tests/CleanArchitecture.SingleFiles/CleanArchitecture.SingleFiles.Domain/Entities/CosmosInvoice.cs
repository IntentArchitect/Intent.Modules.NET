using System;
using System.Collections.Generic;
using CleanArchitecture.SingleFiles.Domain.Common;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.SingleFiles.Domain.Entities
{
    public class CosmosInvoice : IHasDomainEvent
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Description { get; set; }

        public ICollection<CosmosLine> CosmosLines { get; set; } = new List<CosmosLine>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}