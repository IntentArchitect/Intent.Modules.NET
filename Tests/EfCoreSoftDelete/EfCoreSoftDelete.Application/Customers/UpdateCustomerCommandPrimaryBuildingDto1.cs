using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
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