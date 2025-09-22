using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }

        public static CustomerDto Create(Guid id, string name, string surname, DateTime dateOfBirth)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                DateOfBirth = dateOfBirth
            };
        }
    }
}