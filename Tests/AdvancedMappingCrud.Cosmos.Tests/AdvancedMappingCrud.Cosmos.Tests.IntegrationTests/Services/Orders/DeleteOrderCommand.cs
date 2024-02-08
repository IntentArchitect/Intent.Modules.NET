using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Orders
{
    public class DeleteOrderCommand
    {
        public DeleteOrderCommand()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static DeleteOrderCommand Create(string id)
        {
            return new DeleteOrderCommand
            {
                Id = id
            };
        }
    }
}