using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Domain.Configuration
{
    /// <summary>
    /// Provides a hint about how to interpret `Value`.
    /// </summary>
    public enum ConfigurationValueType
    {
        String,
        Int,
        Decimal,
        Bool,
        Json,
        Secret
    }
}