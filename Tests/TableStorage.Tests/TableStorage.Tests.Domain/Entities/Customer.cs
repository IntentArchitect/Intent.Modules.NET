using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace TableStorage.Tests.Domain.Entities
{
    public class Customer
    {
        public string Name { get; set; }
    }
}