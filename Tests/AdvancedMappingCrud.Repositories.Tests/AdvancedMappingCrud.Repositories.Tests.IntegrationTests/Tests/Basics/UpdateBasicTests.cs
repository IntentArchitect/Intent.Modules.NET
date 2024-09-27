using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Basics;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Basics;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UpdateBasicTests : BaseIntegrationTest
    {
        public UpdateBasicTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateBasic_ShouldUpdateBasic()
        {
            // Arrange
            var client = new BasicsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var basicId = await dataFactory.CreateBasic();

            var command = dataFactory.CreateCommand<UpdateBasicCommand>();
            command.Id = basicId;

            // Act
            await client.UpdateBasicAsync(basicId, command);

            // Assert
            var basic = await client.GetBasicByIdAsync(basicId);
            Assert.NotNull(basic);
            Assert.Equal(command.Name, basic.Name);
        }
    }
}