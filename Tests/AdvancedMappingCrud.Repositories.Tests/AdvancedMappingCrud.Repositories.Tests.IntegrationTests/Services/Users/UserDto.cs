using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class UserDto
    {
        public UserDto()
        {
            Email = null!;
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public Guid QuoteId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static UserDto Create(Guid id, string email, Guid quoteId, string name, string surname)
        {
            return new UserDto
            {
                Id = id,
                Email = email,
                QuoteId = quoteId,
                Name = name,
                Surname = surname
            };
        }
    }
}