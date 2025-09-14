using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Kafka.Producer.Domain.Common;
using Kafka.Producer.Domain.Events;

namespace Kafka.Producer.Domain.Entities
{
    public class Invoice : IHasDomainEvent
    {
        public Invoice(string note)
        {
            Note = note;
            DomainEvents.Add(new InvoiceCreated(
                invoice: this));
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Invoice()
        {
            Note = null!;
        }

        public Guid Id { get; set; }

        public string Note { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void Update(string note)
        {
            Note = note;
            DomainEvents.Add(new InvoiceUpdated(
                invoice: this));
        }
    }
}