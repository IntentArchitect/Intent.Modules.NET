using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers
{
    public class CustomerLeanDto
    {
        public CustomerLeanDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static CustomerLeanDto Create(Guid id, string name)
        {
            return new CustomerLeanDto
            {
                Id = id,
                Name = name
            };
        }
    }
}