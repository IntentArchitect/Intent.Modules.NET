using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.ValueObjects
{
    public class KeyValuePairSerialized : ValueObject
    {
        protected KeyValuePairSerialized()
        {
        }

        public KeyValuePairSerialized(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Key;
            yield return Value;
        }
    }
}