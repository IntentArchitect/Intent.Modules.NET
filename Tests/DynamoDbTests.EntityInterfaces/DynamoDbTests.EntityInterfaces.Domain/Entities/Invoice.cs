using DynamoDbTests.EntityInterfaces.Domain.Common;
using DynamoDbTests.EntityInterfaces.Domain.Entities.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.EntityInterfaces.Domain.Entities
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

        public ICollection<LineItem> LineItems { get; set; } = [];

        public InvoiceLogo InvoiceLogo { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        ICollection<ILineItem> IInvoice.LineItems
        {
            get => LineItems.CreateWrapper<ILineItem, LineItem>();
            set => LineItems = value.Cast<LineItem>().ToList();
        }

        IInvoiceLogo IInvoice.InvoiceLogo
        {
            get => InvoiceLogo;
            set => InvoiceLogo = (InvoiceLogo)value;
        }

        public void Update(DateTime date, string number, string clientIdentifier)
        {
            Date = date;
            Number = number;
            ClientIdentifier = clientIdentifier;
        }
    }
}