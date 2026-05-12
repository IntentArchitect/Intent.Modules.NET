using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace EventingSubscribers.Domain
{
    /// <summary>
    /// Enum for Order status values
    /// </summary>
    public enum OrderStatus
    {
        Pending,
        Active,
        Completed
    }
}