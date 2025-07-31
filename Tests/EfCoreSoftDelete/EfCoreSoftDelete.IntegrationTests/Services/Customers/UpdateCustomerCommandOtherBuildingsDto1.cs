using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class UpdateCustomerCommandOtherBuildingsDto1
    {
        public UpdateCustomerCommandOtherBuildingsDto1()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static UpdateCustomerCommandOtherBuildingsDto1 Create(string name)
        {
            return new UpdateCustomerCommandOtherBuildingsDto1
            {
                Name = name
            };
        }
    }
}