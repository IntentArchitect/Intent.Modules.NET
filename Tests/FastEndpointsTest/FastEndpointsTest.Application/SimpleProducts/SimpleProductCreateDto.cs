using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.SimpleProducts
{
    public record SimpleProductCreateDto
    {
        public SimpleProductCreateDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; init; }
        public string Value { get; init; }

        public static SimpleProductCreateDto Create(string name, string value)
        {
            return new SimpleProductCreateDto
            {
                Name = name,
                Value = value
            };
        }
    }
}