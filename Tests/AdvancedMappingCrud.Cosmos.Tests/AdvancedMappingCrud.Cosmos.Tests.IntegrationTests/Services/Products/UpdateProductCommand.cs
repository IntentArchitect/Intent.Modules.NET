using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Products
{
    public class UpdateProductCommand
    {
        public UpdateProductCommand()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public static UpdateProductCommand Create(string id, string name)
        {
            return new UpdateProductCommand
            {
                Id = id,
                Name = name
            };
        }
    }
}