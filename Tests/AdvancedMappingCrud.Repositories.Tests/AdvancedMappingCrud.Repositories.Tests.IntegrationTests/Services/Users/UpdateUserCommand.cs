using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class UpdateUserCommand
    {
        public UpdateUserCommand()
        {
            Email = null!;
            Name = null!;
            Surname = null!;
            Line1 = null!;
            Line2 = null!;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public Guid QuoteId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }

        public static UpdateUserCommand Create(
            Guid id,
            string email,
            Guid quoteId,
            string name,
            string surname,
            string line1,
            string line2)
        {
            return new UpdateUserCommand
            {
                Id = id,
                Email = email,
                QuoteId = quoteId,
                Name = name,
                Surname = surname,
                Line1 = line1,
                Line2 = line2
            };
        }
    }
}