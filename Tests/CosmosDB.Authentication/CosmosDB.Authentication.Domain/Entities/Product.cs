using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CosmosDB.Authentication.Domain.Entities
{
    public class Product
    {
        private string? _id;
        private string? _name;

        public Product()
        {
            Id = null!;
            Name = null!;
            Value = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name
        {
            get => _name ??= Guid.NewGuid().ToString();
            set => _name = value;
        }

        public string Value { get; set; }

        public int Qty { get; set; }
    }
}