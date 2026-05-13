using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EventingSubscribers.Domain.Entities
{
    /// <summary>
    /// Entity with operation/method call
    /// </summary>
    public class Account
    {
        public Account()
        {
            Tier = null!;
        }

        public Guid Id { get; set; }

        public string Tier { get; set; }

        public void CompleteUpgrade()
        {
            // TODO: Implement CompleteUpgrade (Account) functionality
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}