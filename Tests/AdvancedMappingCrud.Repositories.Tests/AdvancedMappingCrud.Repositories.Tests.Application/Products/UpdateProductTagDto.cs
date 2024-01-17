using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    public class UpdateProductTagDto
    {
        public UpdateProductTagDto()
        {
            Name = null!;
            Value = null!;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public static UpdateProductTagDto Create(string name, string value)
        {
            return new UpdateProductTagDto
            {
                Name = name,
                Value = value
            };
        }
    }
}