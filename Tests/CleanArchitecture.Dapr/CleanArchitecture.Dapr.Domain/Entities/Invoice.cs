using System;
using System.Collections.Generic;
using CleanArchitecture.Dapr.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Dapr.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Invoice : IHasDomainEvent
    {
        private string? _id;
        public Invoice()
        {
            Id = null!;
            ClientId = null!;
        }
        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public int Number { get; set; }

        public string ClientId { get; set; }

        public ICollection<InvoiceLine> InvoiceLines { get; set; } = [];

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}