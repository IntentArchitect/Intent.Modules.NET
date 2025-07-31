using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
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