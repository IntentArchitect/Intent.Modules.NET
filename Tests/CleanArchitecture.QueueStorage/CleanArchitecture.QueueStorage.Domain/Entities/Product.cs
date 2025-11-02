using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.QueueStorage.Domain.Entities
{
    public class Product
    {
        public Product()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Qty { get; set; }
    }
}