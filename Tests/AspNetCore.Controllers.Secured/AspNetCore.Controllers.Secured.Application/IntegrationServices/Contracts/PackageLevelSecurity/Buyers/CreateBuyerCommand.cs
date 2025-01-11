using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AspNetCore.Controllers.Secured.Application.IntegrationServices.Contracts.PackageLevelSecurity.Buyers
{
    public class CreateBuyerCommand
    {
        public CreateBuyerCommand()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public static CreateBuyerCommand Create(string name, string surname, string email)
        {
            return new CreateBuyerCommand
            {
                Name = name,
                Surname = surname,
                Email = email
            };
        }
    }
}