using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.ManyToMany
{
    public class UpdateProductItemDto
    {
        public UpdateProductItemDto()
        {
            Name = null!;
            TagIds = null!;
            CategoryIds = null!;
        }

        public string Name { get; set; }
        public List<Guid> TagIds { get; set; }
        public List<Guid> CategoryIds { get; set; }

        public static UpdateProductItemDto Create(string name, List<Guid> tagIds, List<Guid> categoryIds)
        {
            return new UpdateProductItemDto
            {
                Name = name,
                TagIds = tagIds,
                CategoryIds = categoryIds
            };
        }
    }
}