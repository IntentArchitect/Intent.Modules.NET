using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.Configuration
{
    /// <summary>
    /// Indicates where a configuration entry applies.
    /// </summary>
    public enum ConfigurationScope
    {
        Global,
        Tenant,
        Environment,
        User,
        Service
    }
}