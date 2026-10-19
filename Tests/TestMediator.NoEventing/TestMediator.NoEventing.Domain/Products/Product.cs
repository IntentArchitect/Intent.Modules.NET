using Intent.RoslynWeaver.Attributes;
using TestMediator.NoEventing.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TestMediator.NoEventing.Domain.Products
{
    public class Product : IHasDomainEvent
    {
        public Product(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A product needs a name.", nameof(name));
            }

            EnsurePositive(price, nameof(price));
            Name = name.Trim();
            Price = price;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Product()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];

        public void ChangePrice(decimal newPrice)
        {
            EnsurePositive(newPrice, nameof(newPrice));
            if (newPrice == Price)
            {
                return;
            }

            DomainEvents.Add(new ProductPriceChangedDomainEvent(
                productId: this.Id,
                oldPrice: this.Price,
                newPrice: newPrice));
            Price = newPrice;
        }

        private static void EnsurePositive(decimal price, string paramName)
        {
            if (price <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, price, "A product's price must be greater than zero.");
            }
        }
    }
}
