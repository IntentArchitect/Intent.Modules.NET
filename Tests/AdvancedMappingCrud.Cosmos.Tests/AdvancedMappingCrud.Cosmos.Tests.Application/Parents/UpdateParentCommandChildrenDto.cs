using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    public class UpdateParentCommandChildrenDto
    {
        public UpdateParentCommandChildrenDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public static UpdateParentCommandChildrenDto Create(string id, string name, int age)
        {
            return new UpdateParentCommandChildrenDto
            {
                Id = id,
                Name = name,
                Age = age
            };
        }
    }
}