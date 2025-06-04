using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Products
{
    public class ProductUpdateDto
    {
        public ProductUpdateDto()
        {
            Name = null!;
            Description = null!;
            ImageUrl = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public static ProductUpdateDto Create(Guid id, string name, string description, decimal price, string imageUrl)
        {
            return new ProductUpdateDto
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                ImageUrl = imageUrl
            };
        }
    }
}