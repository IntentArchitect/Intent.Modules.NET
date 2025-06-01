using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain
{
    public class EmbeddedChild : ValueObject
    {
        public EmbeddedChild(string name, int age)
        {
            Name = name;
            Age = age;
        }

        [IntentMerge]
        protected EmbeddedChild()
        {
            Name = null!;
        }

        public string Name { get; private set; }
        public int Age { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Name;
            yield return Age;
        }
    }
}