using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
{
    public class CreateCustomerCommandOtherBuildingsDto1
    {
        public CreateCustomerCommandOtherBuildingsDto1()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateCustomerCommandOtherBuildingsDto1 Create(string name)
        {
            return new CreateCustomerCommandOtherBuildingsDto1
            {
                Name = name
            };
        }
    }
}