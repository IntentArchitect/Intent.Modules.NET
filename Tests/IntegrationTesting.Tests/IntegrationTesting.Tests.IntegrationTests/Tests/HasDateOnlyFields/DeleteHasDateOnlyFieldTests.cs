using System.Net;
using AutoFixture;
using IntegrationTesting.Tests.IntegrationTests.HttpClients;
using IntegrationTesting.Tests.IntegrationTests.HttpClients.HasDateOnlyField.HasDateOnlyFields;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteHasDateOnlyFieldTests : BaseIntegrationTest
    {
        public DeleteHasDateOnlyFieldTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteHasDateOnlyField_ShouldDeleteHasDateOnlyField()
        {
            // Arrange
            var client = new HasDateOnlyFieldsHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var hasDateOnlyFieldId = await dataFactory.CreateHasDateOnlyField();

            // Act
            await client.DeleteHasDateOnlyFieldAsync(hasDateOnlyFieldId);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetHasDateOnlyFieldByIdAsync(hasDateOnlyFieldId));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}