using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Kafka.Consumer.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Kafka.Consumer.Domain.Entities
{
    public class Invoice : IHasDomainEvent
    {
        public Invoice(Guid id, string note)
        {
            Id = id;
            Note = note;
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

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Update(string note)
        {
            Note = note;
        }
    }
}