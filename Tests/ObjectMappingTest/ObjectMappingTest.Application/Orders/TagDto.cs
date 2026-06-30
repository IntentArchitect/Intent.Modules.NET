using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ObjectMappingTest.Application.Orders
{
    public record TagDto
    {
        public TagDto()
        {
            Name = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
    }
}