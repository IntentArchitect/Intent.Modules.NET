using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace EfCoreSoftDelete.IntegrationTests.Tests.Customers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetDeletedCustomerByIdTests : BaseIntegrationTest
    {
        public GetDeletedCustomerByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
    }
}