using System.Net;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.FileUploads;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.FileUploads;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class RestrictedUploadTests : BaseIntegrationTest
    {
        public RestrictedUploadTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }


        [Fact]
        public async Task RestrictedUpload_ShouldThrowInvalidMimeType()
        {
            //Arrange
            var client = new FileUploadsHttpClient(CreateClient());
            string contentType = "application/octet-stream";
            var content = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            //Act
            var act = async () => await client.RestrictedUploadAsync(contentType, null, new Services.FileUploads.RestrictedUploadCommand() { Content = new MemoryStream(content), ContentType = contentType, Filename = "test.txt" });

            //Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(async () => await act());
            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Contains("Invalid file type", exception.Message);
        }
    }
}