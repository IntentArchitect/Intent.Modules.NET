using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers
{
    public class UpdateCustomerCommandOtherBuildingsDto1
    {
        public UpdateCustomerCommandOtherBuildingsDto1()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static UpdateCustomerCommandOtherBuildingsDto1 Create(string name)
        {
            return new UpdateCustomerCommandOtherBuildingsDto1
            {
                Name = name
            };
        }
    }
}