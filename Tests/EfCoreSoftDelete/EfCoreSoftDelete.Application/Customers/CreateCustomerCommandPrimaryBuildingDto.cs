using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
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