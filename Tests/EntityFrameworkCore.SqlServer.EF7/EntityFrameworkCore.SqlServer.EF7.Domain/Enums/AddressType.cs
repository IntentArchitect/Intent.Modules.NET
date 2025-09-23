using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Enums
{
    public enum AddressType
    {
        Postal = 1,
        Billing = 2
    }
}