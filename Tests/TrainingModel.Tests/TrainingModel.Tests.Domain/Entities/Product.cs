using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Domain.Common;
using TrainingModel.Tests.Domain.Services;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TrainingModel.Tests.Domain.Entities
{
    public class Product : IHasDomainEvent
    {
        public Product(string name, Guid brandId, string description)
        {
            Name = name;
            BrandId = brandId;
            Description = description;
            IsActive = false;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Product()
        {
            Name = null!;
            Description = null!;
            Brand = null!;
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid BrandId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

        public virtual ICollection<Price> Prices { get; set; } = new List<Price>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void AddPrice(DateTime activeFrom, decimal price)
        {
            Prices.Add(new Price { ActiveFrom = activeFrom, Amount = price });
        }

        public void Activate()
        {
            if (GetCurrentPrice() <= 0)
            {
                throw new Exception("Product does not have a current price");
            }

            IsActive = true;
        }

        public decimal GetCurrentPrice()
        {
            var currentPrice = Prices
                .Where(p => p.ActiveFrom < DateTime.Now)
                .MaxBy(p => p.ActiveFrom);

            return currentPrice?.Amount ?? -1;
        }

        public async Task CaptureCategoriesAsync(
            IEnumerable<string> categoryNames,
            ICategoryService categoryService,
            CancellationToken cancellationToken = default)
        {
            var categories = await categoryService.GetOrCreateCategoriesAsync(categoryNames, cancellationToken);

            Categories = categories.ToList();
        }

        public void DeActivate()
        {
            IsActive = false;
        }
    }
}