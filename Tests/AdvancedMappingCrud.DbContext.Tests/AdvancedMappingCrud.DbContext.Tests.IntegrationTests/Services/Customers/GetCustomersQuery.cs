using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.Customers
{
    public class GetCustomersQuery
    {
        public static GetCustomersQuery Create()
        {
            return new GetCustomersQuery
            {
            };
        }
    }
}