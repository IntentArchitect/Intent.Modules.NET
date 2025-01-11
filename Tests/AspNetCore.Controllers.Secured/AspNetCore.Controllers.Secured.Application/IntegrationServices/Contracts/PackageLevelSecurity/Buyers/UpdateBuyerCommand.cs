using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.IntegrationServices.Contracts.PackageLevelSecurity.Buyers
{
    public class UpdateBuyerCommand
    {
        public UpdateBuyerCommand()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public Guid Id { get; set; }

        public static UpdateBuyerCommand Create(string name, string surname, string email, Guid id)
        {
            return new UpdateBuyerCommand
            {
                Name = name,
                Surname = surname,
                Email = email,
                Id = id
            };
        }
    }
}