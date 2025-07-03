using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.PartialCruds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests.PartialCruds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetPartialCrudByIdTests : BaseIntegrationTest
    {
        public GetPartialCrudByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetPartialCrudById_ShouldGetPartialCrudById()
        {
            // Arrange
            var client = new PartialCrudsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var partialCrudId = await dataFactory.CreatePartialCrud();

            // Act
            var partialCrud = await client.GetPartialCrudByIdAsync(partialCrudId, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(partialCrud);
        }
    }
}