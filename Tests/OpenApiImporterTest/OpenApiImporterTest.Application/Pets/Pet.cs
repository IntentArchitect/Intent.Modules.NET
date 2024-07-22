using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets
{
    public class Pet
    {
        public Pet()
        {
            Name = null!;
            PhotoUrls = null!;
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public Category? Category { get; set; }
        public List<string> PhotoUrls { get; set; }
        public List<Tag>? Tags { get; set; }
        public StatusType? Status { get; set; }

        public static Pet Create(
            int? id,
            string name,
            Category? category,
            List<string> photoUrls,
            List<Tag>? tags,
            StatusType? status)
        {
            return new Pet
            {
                Id = id,
                Name = name,
                Category = category,
                PhotoUrls = photoUrls,
                Tags = tags,
                Status = status
            };
        }
    }
}