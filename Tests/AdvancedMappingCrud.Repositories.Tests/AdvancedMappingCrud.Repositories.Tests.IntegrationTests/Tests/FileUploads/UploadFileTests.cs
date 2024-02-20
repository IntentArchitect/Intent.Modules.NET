using System.Text;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.FileUploads;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Orders;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class UploadFileTests : BaseIntegrationTest
    {
        public UploadFileTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UploadFile_ShouldUploadFile()
        {
            //Arrange
            var client = new FileUploadsHttpClient(CreateClient());
            string contentType = "text/plain";//"application/octet-stream";
            string content = @"
My File Content
In Here
";

            //Act
            var uploadId = await client.UploadFileAsync(contentType, null, new Services.FileUploads.UploadFileCommand() { Content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)), ContentType = contentType, Filename = "test.txt" });

            //Assert
            var download = await client.DownloadFileAsync(uploadId);
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