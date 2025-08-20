using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Ones;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.Ones
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateOneTests : BaseIntegrationTest
    {
        public CreateOneTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateOne_ShouldCreateOne()
        {
            // Arrange
            var client = new OnesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateOneCommand>();

            // Act
            var oneId = await client.CreateOneAsync(command, TestContext.Current.CancellationToken);

            // Assert
            var one = await client.GetOneByIdAsync(oneId, TestContext.Current.CancellationToken);
            Assert.NotNull(one);
        }
    }
}