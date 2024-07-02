using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.DtoReturns;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetDtoReturnByIdTests : BaseIntegrationTest
    {
        public GetDtoReturnByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetDtoReturnById_ShouldGetDtoReturnById()
        {
            // Arrange
            var client = new DtoReturnsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var dtoReturnId = await dataFactory.CreateDtoReturn();

            // Act
            var dtoReturn = await client.GetDtoReturnByIdAsync(dtoReturnId);

            // Assert
            Assert.NotNull(dtoReturn);
        }
    }
}