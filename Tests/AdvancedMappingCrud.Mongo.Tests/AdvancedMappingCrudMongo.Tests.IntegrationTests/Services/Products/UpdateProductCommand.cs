using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Products
{
    public class UpdateProductCommand
    {
        public UpdateProductCommand()
        {
            Name = null!;
            Description = null!;
            Id = null!;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }

        public static UpdateProductCommand Create(string name, string description, string id)
        {
            return new UpdateProductCommand
            {
                Name = name,
                Description = description,
                Id = id
            };
        }
    }
}