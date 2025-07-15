using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Application.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
            Suranme = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Suranme { get; set; }

        public static CustomerDto Create(Guid id, string name, string suranme)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Suranme = suranme
            };
        }
    }
}