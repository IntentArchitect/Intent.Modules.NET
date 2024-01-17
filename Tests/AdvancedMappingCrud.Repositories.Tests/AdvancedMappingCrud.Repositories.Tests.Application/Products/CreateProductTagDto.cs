using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    public class CreateProductTagDto
    {
        public CreateProductTagDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public static CreateProductTagDto Create(string name, string value)
        {
            return new CreateProductTagDto
            {
                Name = name,
                Value = value
            };
        }
    }
}