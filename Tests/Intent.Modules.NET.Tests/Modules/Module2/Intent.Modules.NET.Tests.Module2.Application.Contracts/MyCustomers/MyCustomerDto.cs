using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers
{
    public class MyCustomerDto
    {
        public MyCustomerDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static MyCustomerDto Create(Guid id, string name)
        {
            return new MyCustomerDto
            {
                Id = id,
                Name = name
            };
        }
    }
}