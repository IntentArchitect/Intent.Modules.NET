using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.CheckNewCompChildCruds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests.CheckNewCompChildCruds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetCheckNewCompChildCrudByIdTests : BaseIntegrationTest
    {
        public GetCheckNewCompChildCrudByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetCheckNewCompChildCrudById_ShouldGetCheckNewCompChildCrudById()
        {
            // Arrange
            var client = new CheckNewCompChildCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var checkNewCompChildCrudId = await dataFactory.CreateCheckNewCompChildCrud();

            // Act
            var checkNewCompChildCrud = await client.GetCheckNewCompChildCrudByIdAsync(checkNewCompChildCrudId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(checkNewCompChildCrud);
        }
    }
}