using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.DtoReturns;
using IntegrationTesting.Tests.IntegrationTests.Services.DtoReturns;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateDtoReturnTests : BaseIntegrationTest
    {
        public CreateDtoReturnTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateDtoReturn_ShouldCreateDtoReturn()
        {
            // Arrange
            var integrationClient = new DtoReturnsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateDtoReturnCommand>();

            // Act
            var createdDto = await integrationClient.CreateDtoReturnAsync(command);
            var dtoReturnId = createdDto.Id;

            // Assert
            var dtoReturn = await integrationClient.GetDtoReturnByIdAsync(dtoReturnId);
            Assert.NotNull(dtoReturn);
        }
    }
}