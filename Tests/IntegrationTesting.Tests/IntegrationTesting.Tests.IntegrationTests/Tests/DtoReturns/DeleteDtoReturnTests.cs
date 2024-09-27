using System.Net;
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
    public class DeleteDtoReturnTests : BaseIntegrationTest
    {
        public DeleteDtoReturnTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteDtoReturn_ShouldDeleteDtoReturn()
        {
            // Arrange
            var client = new DtoReturnsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var dtoReturnId = await dataFactory.CreateDtoReturn();

            // Act
            await client.DeleteDtoReturnAsync(dtoReturnId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetDtoReturnByIdAsync(dtoReturnId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}