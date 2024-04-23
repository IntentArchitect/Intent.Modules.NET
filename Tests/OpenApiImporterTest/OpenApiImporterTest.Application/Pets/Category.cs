using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Pets
{
    public class Category
    {
        public Category()
        {
            Name = null!;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public static Category Create(int id, string name)
        {
            return new Category
            {
                Id = id,
                Name = name
            };
        }
    }
}