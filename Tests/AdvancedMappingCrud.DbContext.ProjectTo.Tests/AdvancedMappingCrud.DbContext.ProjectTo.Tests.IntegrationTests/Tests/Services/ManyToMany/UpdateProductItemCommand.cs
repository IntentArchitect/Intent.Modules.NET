using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.Tests.Services.ManyToMany
{
    public class UpdateProductItemCommand
    {
        public UpdateProductItemCommand()
        {
            Name = null!;
            TagIds = null!;
            CategoryIds = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> TagIds { get; set; }
        public List<Guid> CategoryIds { get; set; }

        public static UpdateProductItemCommand Create(Guid id, string name, List<Guid> tagIds, List<Guid> categoryIds)
        {
            return new UpdateProductItemCommand
            {
                Id = id,
                Name = name,
                TagIds = tagIds,
                CategoryIds = categoryIds
            };
        }
    }
}