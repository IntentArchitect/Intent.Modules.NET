using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Optionals;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Optionals;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateOptionalTests : BaseIntegrationTest
    {
        public CreateOptionalTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateOptional_ShouldCreateOptional()
        {
            // Arrange
            var client = new OptionalsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateOptionalCommand>();

            // Act
            var optionalId = await client.CreateOptionalAsync(command);

            // Assert
            var optional = await client.GetOptionalByIdAsync(optionalId);
            Assert.NotNull(optional);
        }
    }
}