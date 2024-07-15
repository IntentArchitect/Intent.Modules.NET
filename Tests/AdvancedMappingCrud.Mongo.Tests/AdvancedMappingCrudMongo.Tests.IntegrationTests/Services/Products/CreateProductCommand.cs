using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Products
{
    public class CreateProductCommand
    {
        public CreateProductCommand()
        {
            Name = null!;
            Description = null!;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public static CreateProductCommand Create(string name, string description)
        {
            return new CreateProductCommand
            {
                Name = name,
                Description = description
            };
        }
    }
}