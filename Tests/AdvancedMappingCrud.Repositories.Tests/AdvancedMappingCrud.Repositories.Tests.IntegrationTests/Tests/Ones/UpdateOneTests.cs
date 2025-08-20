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
    public class UpdateOneTests : BaseIntegrationTest
    {
        public UpdateOneTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateOne_ShouldUpdateOne()
        {
            // Arrange
            var client = new OnesHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var oneId = await dataFactory.CreateOne();

            var command = dataFactory.CreateCommand<UpdateOneCommand>();
            command.Id = oneId;

            // Act
            await client.UpdateOneAsync(oneId, command, TestContext.Current.CancellationToken);

            // Assert
            var one = await client.GetOneByIdAsync(oneId, TestContext.Current.CancellationToken);
            Assert.NotNull(one);
        }
    }
}