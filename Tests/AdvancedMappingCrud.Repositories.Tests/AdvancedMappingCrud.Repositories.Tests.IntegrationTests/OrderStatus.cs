using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.EnumContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests
{
    public enum OrderStatus
    {
        Paid = 1,
        Shipped = 2
    }
}