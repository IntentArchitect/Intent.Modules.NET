using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.SimpleProducts
{
    public class SimpleProductUpdateDto
    {
        public SimpleProductUpdateDto()
        {
            Name = null!;
            Value = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public static SimpleProductUpdateDto Create(Guid id, string name, string value)
        {
            return new SimpleProductUpdateDto
            {
                Id = id,
                Name = name,
                Value = value
            };
        }
    }
}