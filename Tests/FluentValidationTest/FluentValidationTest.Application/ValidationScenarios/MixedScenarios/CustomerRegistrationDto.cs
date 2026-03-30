using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios
{
    public record CustomerRegistrationDto
    {
        public CustomerRegistrationDto()
        {
            FirstName = null!;
            LastName = null!;
            Email = null!;
            Username = null!;
        }

        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Username { get; init; }
        public string? WebsiteUrl { get; init; }
    }
}