using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
{
    public class UpdateCustomerCommandOtherBuildingsDto
    {
        public UpdateCustomerCommandOtherBuildingsDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static UpdateCustomerCommandOtherBuildingsDto Create(string name)
        {
            return new UpdateCustomerCommandOtherBuildingsDto
            {
                Name = name
            };
        }
    }
}