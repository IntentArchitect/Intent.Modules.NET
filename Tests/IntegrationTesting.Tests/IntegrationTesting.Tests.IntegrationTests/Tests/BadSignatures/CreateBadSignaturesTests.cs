using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.BadSignatures;
using IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests.BadSignatures
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateBadSignaturesTests : BaseIntegrationTest
    {
        public CreateBadSignaturesTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateBadSignatures_ShouldCreateBadSignatures()
        {
            // Arrange
            var client = new BadSignaturesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateBadSignaturesCommand>();

            // Act
            var badSignaturesId = await client.CreateBadSignaturesAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var badSignatures = await client.GetBadSignaturesByIdAsync(badSignaturesId, TestContext.Current.CancellationToken);
            Assert.NotNull(badSignatures);
        }
    }
}