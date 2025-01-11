using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.IntegrationServices.Contracts.PackageLevelSecurity.Buyers
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

        public static BuyerDto Create(Guid id, string name, string surname, string email)
        {
            return new BuyerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                Email = email
            };
        }
    }
}