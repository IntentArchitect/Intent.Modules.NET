using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Entities
{
    public class Customer : Person
    {
        public string Status { get; private set; }
    }
}