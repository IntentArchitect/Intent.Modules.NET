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
    public class CreateBasicTests : BaseIntegrationTest
    {
        public CreateBasicTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateBasic_ShouldCreateBasic()
        {
            // Arrange
            var client = new BasicsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateBasicCommand>();

            // Act
            var basicId = await client.CreateBasicAsync(command);

            // Assert
            var basic = await client.GetBasicByIdAsync(basicId);
            Assert.NotNull(basic);
        }
    }
}