using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace CosmosDB.EnumStrings.Domain
{
    public class EmbeddedObject : ValueObject
    {
        protected EmbeddedObject()
        {
        }

        public EmbeddedObject(string name, EnumExample enumExample, EnumExample? nullableEnumExample)
        {
            Name = name;
            EnumExample = enumExample;
            NullableEnumExample = nullableEnumExample;
        }

        public string Name { get; private set; }
        public EnumExample EnumExample { get; private set; }
        public EnumExample? NullableEnumExample { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Name;
            yield return EnumExample;
            yield return NullableEnumExample;
        }
    }
}