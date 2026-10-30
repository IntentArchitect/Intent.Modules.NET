using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Domain.Configuration
{
    public class ConfigurationScopeKey : ValueObject
    {
        public ConfigurationScopeKey(ConfigurationScope scope, string? scopeId)
        {
            Scope = scope;
            ScopeId = scopeId;
        }

        protected ConfigurationScopeKey()
        {
        }

        public ConfigurationScope Scope { get; private set; }
        public string? ScopeId { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Scope;
            yield return ScopeId!;
        }
    }
}