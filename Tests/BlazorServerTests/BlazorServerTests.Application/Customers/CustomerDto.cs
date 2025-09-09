using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace BlazorServerTests.Application.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }

        public static CustomerDto Create(Guid id, string name, string surname, string email, bool isActive)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                Email = email,
                IsActive = isActive
            };
        }
    }
}