using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class CreateCustomerCommandPrimaryBuildingDto1
    {
        public CreateCustomerCommandPrimaryBuildingDto1()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateCustomerCommandPrimaryBuildingDto1 Create(string name)
        {
            return new CreateCustomerCommandPrimaryBuildingDto1
            {
                Name = name
            };
        }
    }
}