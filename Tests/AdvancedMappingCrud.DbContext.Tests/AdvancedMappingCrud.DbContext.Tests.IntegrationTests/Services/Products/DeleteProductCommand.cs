using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Products
{
    public class DeleteProductCommand
    {
        public Guid Id { get; set; }

        public static DeleteProductCommand Create(Guid id)
        {
            return new DeleteProductCommand
            {
                Id = id
            };
        }
    }
}