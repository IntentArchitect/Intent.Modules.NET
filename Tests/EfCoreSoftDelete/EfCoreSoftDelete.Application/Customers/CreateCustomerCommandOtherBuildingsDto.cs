using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
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