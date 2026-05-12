using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EventingSubscribers.Domain.Entities
{
    /// <summary>
    /// Entity with constructor/static factory
    /// </summary>
    public class Item
    {
        public Item()
        {
            Category = null!;
        }

        public Guid Id { get; set; }

        public string Category { get; set; }
    }
}