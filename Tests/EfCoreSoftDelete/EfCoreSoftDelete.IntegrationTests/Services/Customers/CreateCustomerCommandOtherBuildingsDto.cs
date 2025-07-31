using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace EfCoreSoftDelete.IntegrationTests.Services.Customers
{
    public class CreateCustomerCommandOtherBuildingsDto
    {
        public CreateCustomerCommandOtherBuildingsDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateCustomerCommandOtherBuildingsDto Create(string name)
        {
            return new CreateCustomerCommandOtherBuildingsDto
            {
                Name = name
            };
        }
    }
}