using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Domain.Entities
{
    public class Invoice : IHasDomainEvent, IAuditable
    {
        private string? _id;

        public Invoice(string clientIdentifier, DateTime date, string number)
        {
            ClientIdentifier = clientIdentifier;
            Date = date;
            Number = number;
        }

        /// <summary>
        /// Required for derived Cosmos DB documents.
        /// </summary>
        protected Invoice()
        {
            Id = null!;
            ClientIdentifier = null!;
            Number = null!;
            CreatedBy = null!;
        }
        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string ClientIdentifier { get; set; }

        public DateTime Date { get; set; }

        public string Number { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }

        public ICollection<LineItem> LineItems { get; set; } = new List<LineItem>();

        public InvoiceLogo InvoiceLogo { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void Update(DateTime date, string number, string clientIdentifier)
        {
            Date = date;
            Number = number;
            ClientIdentifier = clientIdentifier;
        }

        void IAuditable.SetCreated(string createdBy, DateTimeOffset createdDate) => (CreatedBy, CreatedDate) = (createdBy, createdDate);

        void IAuditable.SetUpdated(string updatedBy, DateTimeOffset updatedDate) => (UpdatedBy, UpdatedDate) = (updatedBy, updatedDate);
    }
}