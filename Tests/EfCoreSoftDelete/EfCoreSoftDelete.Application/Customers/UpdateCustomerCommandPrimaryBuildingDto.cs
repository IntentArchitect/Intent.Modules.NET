using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
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