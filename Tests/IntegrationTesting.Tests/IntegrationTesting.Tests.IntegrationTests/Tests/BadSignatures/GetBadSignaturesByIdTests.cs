using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.BadSignatures;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetBadSignaturesByIdTests : BaseIntegrationTest
    {
        public GetBadSignaturesByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetBadSignaturesById_ShouldGetBadSignaturesById()
        {
            // Arrange
            var client = new BadSignaturesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var badSignaturesId = await dataFactory.CreateBadSignatures();

            // Act
            var badSignatures = await client.GetBadSignaturesByIdAsync(badSignaturesId);

            // Assert
            Assert.NotNull(badSignatures);
        }
    }
}