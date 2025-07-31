using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class UpdateCustomerCommandPrimaryBuildingDto
    {
        public UpdateCustomerCommandPrimaryBuildingDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static UpdateCustomerCommandPrimaryBuildingDto Create(string name)
        {
            return new UpdateCustomerCommandPrimaryBuildingDto
            {
                Name = name
            };
        }
    }
}