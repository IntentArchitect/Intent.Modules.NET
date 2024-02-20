using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.FileUploads;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UploadTests : BaseIntegrationTest
    {
        public UploadTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Upload()
        {
            //Arrange
            var client = new UploadDownloadServiceHttpClient(CreateClient());
            string contentType = "text/plain";//"application/octet-stream";
            string content = @"
My File Content
In Here
";

            //Act
            var uploadId = await client.UploadAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)), null, contentType, null);

            //Assert
            var download = await client.DownloadAsync(uploadId);
            Assert.NotNull(download);
            using (StreamReader reader = new StreamReader(download.Content))
            {
                // Process the stream as needed
                string result = await reader.ReadToEndAsync();
                Assert.Equal(content, result);
            }
        }

    }
}