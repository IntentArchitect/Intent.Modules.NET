using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class UpdateCustomerCommandPrimaryBuildingDto1
    {
        public UpdateCustomerCommandPrimaryBuildingDto1()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static UpdateCustomerCommandPrimaryBuildingDto1 Create(string name)
        {
            return new UpdateCustomerCommandPrimaryBuildingDto1
            {
                Name = name
            };
        }
    }
}