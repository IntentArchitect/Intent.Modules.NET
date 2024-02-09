using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products
{
    public class CreateProductCommand
    {
        public CreateProductCommand()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public decimal Price { get; set; }

        public static CreateProductCommand Create(string name, decimal price)
        {
            return new CreateProductCommand
            {
                Name = name,
                Price = price
            };
        }
    }
}