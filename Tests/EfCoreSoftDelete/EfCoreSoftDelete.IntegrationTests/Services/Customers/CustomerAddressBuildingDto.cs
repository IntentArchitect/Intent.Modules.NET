using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class CustomerAddressBuildingDto
    {
        public CustomerAddressBuildingDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CustomerAddressBuildingDto Create(string name)
        {
            return new CustomerAddressBuildingDto
            {
                Name = name
            };
        }
    }
}