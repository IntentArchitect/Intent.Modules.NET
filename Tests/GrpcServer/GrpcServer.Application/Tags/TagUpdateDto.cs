using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.Tags
{
    public class TagUpdateDto
    {
        public TagUpdateDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static TagUpdateDto Create(Guid id, string name)
        {
            return new TagUpdateDto
            {
                Id = id,
                Name = name
            };
        }
    }
}