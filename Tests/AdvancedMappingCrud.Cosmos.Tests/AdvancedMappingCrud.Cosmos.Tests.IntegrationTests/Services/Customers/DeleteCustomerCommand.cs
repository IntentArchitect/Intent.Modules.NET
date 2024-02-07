using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Customers
{
    public class DeleteCustomerCommand
    {
        public DeleteCustomerCommand()
        {
            Id = null!;
        }

        public string Id { get; set; }

        public static DeleteCustomerCommand Create(string id)
        {
            return new DeleteCustomerCommand
            {
                Id = id
            };
        }
    }
}