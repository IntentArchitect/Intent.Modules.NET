using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Products
{
    public class PatchProductTagDto
    {
        public PatchProductTagDto()
        {
        }

        public string? Name { get; set; }
        public string? Value { get; set; }

        public static PatchProductTagDto Create(string? name, string? value)
        {
            return new PatchProductTagDto
            {
                Name = name,
                Value = value
            };
        }
    }
}