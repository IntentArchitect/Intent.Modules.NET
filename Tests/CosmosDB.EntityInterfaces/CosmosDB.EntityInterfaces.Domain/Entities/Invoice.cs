using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.EntityInterfaces.Domain.Common;
using CosmosDB.EntityInterfaces.Domain.Common.Interfaces;
using CosmosDB.EntityInterfaces.Domain.Entities.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public class Invoice : IInvoice, IHasDomainEvent
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

        ICollection<ILineItem> IInvoice.LineItems
        {
            get => LineItems.CreateWrapper<ILineItem, LineItem>();
            set => LineItems = value.Cast<LineItem>().ToList();
        }

        public InvoiceLogo InvoiceLogo { get; set; }

        IInvoiceLogo IInvoice.InvoiceLogo
        {
            get => InvoiceLogo;
            set => InvoiceLogo = (InvoiceLogo)value;
        }

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