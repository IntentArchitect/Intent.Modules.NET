using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.Tests.Products
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetProductsPaginatedByNameOptionalWithOrderTests : BaseIntegrationTest
    {
        public GetProductsPaginatedByNameOptionalWithOrderTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }
    }
}