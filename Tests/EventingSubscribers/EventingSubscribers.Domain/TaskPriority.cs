using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace EventingSubscribers.Domain
{
    /// <summary>
    /// Enum for Task priority values
    /// </summary>
    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }
}