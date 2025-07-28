using DynamoDbTests.PrivateSetters.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.PrivateSetters.Domain.Entities
{
    public class Invoice : IHasDomainEvent
    {
        private List<LineItem> _lineItems = [];
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
            private set => _id = value;
        }

        public string ClientIdentifier { get; private set; }

        public DateTime Date { get; private set; }

        public string Number { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset CreatedDate { get; private set; }

        public string? UpdatedBy { get; private set; }

        public DateTimeOffset? UpdatedDate { get; private set; }

        public IReadOnlyCollection<LineItem> LineItems
        {
            get => _lineItems.AsReadOnly();
            private set => _lineItems = new List<LineItem>(value);
        }

        public InvoiceLogo InvoiceLogo { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void Update(DateTime date, string number, string clientIdentifier)
        {
            Date = date;
            Number = number;
            ClientIdentifier = clientIdentifier;
        }
    }
}