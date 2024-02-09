using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Redis.Om.Repositories.Domain.Entities
{
    public class Invoice : IHasDomainEvent
    {
        private string? _id;
        public Invoice(string clientIdentifier, DateTime date, string number)
        {
            ClientIdentifier = clientIdentifier;
            Date = date;
            Number = number;
        }

        /// <summary>
        /// Required for derived Redis OM documents.
        /// </summary>
        protected Invoice()
        {
            Id = null!;
            Number = null!;
            CreatedBy = null!;
            ClientIdentifier = null!;
        }

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public DateTime Date { get; set; }

        public string Number { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }

        public string ClientIdentifier { get; set; }

        public ICollection<LineItem> LineItems { get; set; } = new List<LineItem>();

        public InvoiceLogo InvoiceLogo { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Update(DateTime date, string number, string clientIdentifier)
        {
            Date = date;
            Number = number;
            ClientIdentifier = clientIdentifier;
        }
    }
}