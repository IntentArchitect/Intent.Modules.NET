using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TestMediator.Consolidated.Application.Customers
{
    public record CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
            Email = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
    }
}