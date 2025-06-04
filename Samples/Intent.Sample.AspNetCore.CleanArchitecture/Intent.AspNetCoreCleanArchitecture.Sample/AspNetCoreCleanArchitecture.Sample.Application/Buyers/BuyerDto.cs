using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers
{
    public class BuyerDto
    {
        public BuyerDto()
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

        public static BuyerDto Create(Guid id, string name, string surname, string email, bool isActive)
        {
            return new BuyerDto
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