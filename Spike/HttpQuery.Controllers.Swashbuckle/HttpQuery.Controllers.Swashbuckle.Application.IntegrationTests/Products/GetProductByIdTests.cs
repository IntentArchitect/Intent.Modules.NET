using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace HttpQuery.Controllers.Swashbuckle.Application.IntegrationTests.Products
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetProductByIdTests : BaseIntegrationTest
    {
        public GetProductByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
    }
}