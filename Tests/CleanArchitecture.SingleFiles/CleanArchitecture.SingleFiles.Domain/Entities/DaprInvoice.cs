using System;
using System.Collections.Generic;
using CleanArchitecture.SingleFiles.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.SingleFiles.Domain.Entities
{
    public class DaprInvoice : IHasDomainEvent
    {
        private string? _id;
        public DaprInvoice()
        {
            Id = null!;
            Description = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Description { get; set; }

        public ICollection<DaprLine> DaprLines { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}