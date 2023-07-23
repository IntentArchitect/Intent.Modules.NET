using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Deriveds
{
    public class DerivedUpdateDto
    {
        public DerivedUpdateDto()
        {
            Id = null!;
            DerivedAttribute = null!;
            BaseAttribute = null!;
        }

        public string Id { get; set; }
        public string DerivedAttribute { get; set; }
        public string BaseAttribute { get; set; }

        public static DerivedUpdateDto Create(string id, string derivedAttribute, string baseAttribute)
        {
            return new DerivedUpdateDto
            {
                Id = id,
                DerivedAttribute = derivedAttribute,
                BaseAttribute = baseAttribute
            };
        }
    }
}