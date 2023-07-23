using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.DerivedOfTS
{
    public class DerivedOfTUpdateDto
    {
        public DerivedOfTUpdateDto()
        {
            Id = null!;
            DerivedAttribute = null!;
        }

        public string Id { get; set; }
        public string DerivedAttribute { get; set; }
        public int BaseAttribute { get; set; }

        public static DerivedOfTUpdateDto Create(string id, string derivedAttribute, int baseAttribute)
        {
            return new DerivedOfTUpdateDto
            {
                Id = id,
                DerivedAttribute = derivedAttribute,
                BaseAttribute = baseAttribute
            };
        }
    }
}