using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Domain
{
    public enum OrderStatus
    {
        Paid = 1,
        Shipped = 2
    }
}