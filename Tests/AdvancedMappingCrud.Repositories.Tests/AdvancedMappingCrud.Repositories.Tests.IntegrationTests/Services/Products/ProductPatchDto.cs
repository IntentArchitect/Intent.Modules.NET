using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Products
{
    public class ProductPatchDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<PatchProductTagDto>? Tags { get; set; }

        public static ProductPatchDto Create(Guid id, string? name, List<PatchProductTagDto>? tags)
        {
            return new ProductPatchDto
            {
                Id = id,
                Name = name,
                Tags = tags
            };
        }
    }
}