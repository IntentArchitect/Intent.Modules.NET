using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.Tags
{
    public class TagCreateDto
    {
        public TagCreateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static TagCreateDto Create(string name)
        {
            return new TagCreateDto
            {
                Name = name
            };
        }
    }
}