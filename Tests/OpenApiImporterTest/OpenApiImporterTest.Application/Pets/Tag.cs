using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets
{
    public class Tag
    {
        public Tag()
        {
            Name = null!;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public static Tag Create(int id, string name)
        {
            return new Tag
            {
                Id = id,
                Name = name
            };
        }
    }
}