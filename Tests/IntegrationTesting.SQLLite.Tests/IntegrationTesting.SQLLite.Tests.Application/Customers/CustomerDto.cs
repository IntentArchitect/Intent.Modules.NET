using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.Customers
{
    public record CustomerDto
    {
        public CustomerDto()
        {
            FirstName = null!;
            LastName = null!;
            Email = null!;
        }

        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string? PhoneNumber { get; init; }
    }
}