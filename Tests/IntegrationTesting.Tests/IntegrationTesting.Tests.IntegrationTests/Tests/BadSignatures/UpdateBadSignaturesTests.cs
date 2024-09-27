using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.BadSignatures;
using IntegrationTesting.Tests.IntegrationTests.Services.BadSignatures;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateBadSignaturesTests : BaseIntegrationTest
    {
        public UpdateBadSignaturesTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateBadSignatures_ShouldUpdateBadSignatures()
        {
            // Arrange
            var client = new BadSignaturesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var badSignaturesId = await dataFactory.CreateBadSignatures();

            var command = dataFactory.CreateCommand<UpdateBadSignaturesCommand>();
            command.Id = badSignaturesId;

            // Act
            await client.UpdateBadSignaturesAsync(badSignaturesId, command);

            // Assert
            var badSignatures = await client.GetBadSignaturesByIdAsync(badSignaturesId);
            Assert.NotNull(badSignatures);
            Assert.Equal(command.Name, badSignatures.Name);
        }
    }
}