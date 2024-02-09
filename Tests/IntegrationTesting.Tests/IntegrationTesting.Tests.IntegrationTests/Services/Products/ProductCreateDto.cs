using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.Products
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