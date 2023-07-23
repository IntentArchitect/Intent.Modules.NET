using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.DerivedOfTS
{
    public class DerivedOfTCreateDto
    {
        public DerivedOfTCreateDto()
        {
            DerivedAttribute = null!;
        }

        public string DerivedAttribute { get; set; }
        public int BaseAttribute { get; set; }

        public static DerivedOfTCreateDto Create(string derivedAttribute, int baseAttribute)
        {
            return new DerivedOfTCreateDto
            {
                DerivedAttribute = derivedAttribute,
                BaseAttribute = baseAttribute
            };
        }
    }
}