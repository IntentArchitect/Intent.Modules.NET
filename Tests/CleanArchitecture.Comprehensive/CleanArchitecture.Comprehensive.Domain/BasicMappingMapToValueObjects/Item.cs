using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.BasicMappingMapToValueObjects
{
    public class Item : ValueObject
    {
        public Item(string key, string value)
        {
            Key = key;
            Value = value;
        }

        [IntentMerge]
        protected Item()
        {
            Key = null!;
            Value = null!;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Key;
            yield return Value;
        }
    }
}