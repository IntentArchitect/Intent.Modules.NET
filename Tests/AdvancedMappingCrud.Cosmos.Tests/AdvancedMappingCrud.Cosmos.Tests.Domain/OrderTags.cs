using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain
{
    public class OrderTags : ValueObject
    {
        protected OrderTags()
        {
        }

        public OrderTags(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Name;
            yield return Value;
        }
    }
}