using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DynamoDbTests.Domain.Entities
{
    public class LineItem
    {
        private string? _id;

        public LineItem()
        {
            Id = null!;
            Description = null!;
            ProductId = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public string ProductId { get; set; }

        public IList<string> Tags { get; set; } = [];
    }
}