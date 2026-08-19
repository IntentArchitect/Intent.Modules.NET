using Intent.RoslynWeaver.Attributes;
using ObjectMapping.Lenient.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMapping.Lenient.Application.Orders
{
    public record CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public CustomerTier Tier { get; init; }
    }
}