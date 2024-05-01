using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Products
{
    public class ProductCreateDto
    {
        public ProductCreateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static ProductCreateDto Create(string name)
        {
            return new ProductCreateDto
            {
                Name = name
            };
        }
    }
}