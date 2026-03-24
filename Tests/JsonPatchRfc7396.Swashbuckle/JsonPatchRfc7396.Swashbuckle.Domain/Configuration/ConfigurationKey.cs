using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.Configuration
{
    public class ConfigurationKey : ValueObject
    {
        public ConfigurationKey(string value)
        {
            Value = value;
        }

        [IntentMerge]
        protected ConfigurationKey()
        {
            Value = null!;
        }

        public string Value { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Value;
        }
    }
}