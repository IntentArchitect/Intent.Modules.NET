using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.SimpleProducts
{
    public class SimpleProductCreateDto
    {
        public SimpleProductCreateDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; set; }
        public string Value { get; set; }

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