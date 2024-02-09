using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class CreateUserCommand
    {
        public CreateUserCommand()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public Guid QuoteId { get; set; }

        public static CreateUserCommand Create(string name, string surname, string email, Guid quoteId)
        {
            return new CreateUserCommand
            {
                Name = name,
                Surname = surname,
                Email = email,
                QuoteId = quoteId
            };
        }
    }
}