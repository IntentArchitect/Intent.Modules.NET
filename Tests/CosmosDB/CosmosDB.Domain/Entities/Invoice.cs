using System;
using System.Collections.Generic;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CosmosDB.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Invoice : IHasDomainEvent, IAuditable
    {
        private string? _id;

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public Invoice(string clientIdentifier, DateTime date, string number)
        {
            ClientIdentifier = clientIdentifier;
            Date = date;
            Number = number;
        }

        /// <summary>
        /// Required for derived Cosmos DB documents.
        /// </summary>
        [IntentManaged(Mode.Fully)]
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

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
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