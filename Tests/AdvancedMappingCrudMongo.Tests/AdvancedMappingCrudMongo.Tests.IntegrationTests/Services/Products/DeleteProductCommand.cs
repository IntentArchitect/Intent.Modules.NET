using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Products
{
    public class DeleteProductCommand
    {
        public DeleteProductCommand()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static DeleteProductCommand Create(string id)
        {
            return new DeleteProductCommand
            {
                Id = id
            };
        }
    }
}