using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class CreateCustomerCommandPrimaryBuildingDto
    {
        public CreateCustomerCommandPrimaryBuildingDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateCustomerCommandPrimaryBuildingDto Create(string name)
        {
            return new CreateCustomerCommandPrimaryBuildingDto
            {
                Name = name
            };
        }
    }
}