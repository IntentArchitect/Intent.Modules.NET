using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
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