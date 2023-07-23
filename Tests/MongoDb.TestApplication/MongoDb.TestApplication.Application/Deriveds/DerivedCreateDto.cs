using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Deriveds
{
    public class DerivedCreateDto
    {
        public DerivedCreateDto()
        {
            DerivedAttribute = null!;
            BaseAttribute = null!;
        }

        public string DerivedAttribute { get; set; }
        public string BaseAttribute { get; set; }

        public static DerivedCreateDto Create(string derivedAttribute, string baseAttribute)
        {
            return new DerivedCreateDto
            {
                DerivedAttribute = derivedAttribute,
                BaseAttribute = baseAttribute
            };
        }
    }
}