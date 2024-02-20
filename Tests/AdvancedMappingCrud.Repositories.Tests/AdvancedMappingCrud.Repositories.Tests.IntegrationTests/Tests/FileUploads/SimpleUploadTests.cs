using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.FileUploads;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.FileUploads;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class SimpleUploadTests : BaseIntegrationTest
    {
        public SimpleUploadTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task SimpleUpload_ShouldUploadFile()
        {
            //Arrange
            var client = new FileUploadsHttpClient(CreateClient());
            var content = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            //Act
            var uploadId = await client.SimpleUploadAsync(new Services.FileUploads.SimpleUploadCommand() { Content = new MemoryStream(content) });

            //Assert
            var download = await client.SimpleDownloadAsync(uploadId);
            Assert.NotNull(download);
            var result = ByteHelper.ReadBytes(download.Content);
            Assert.Equal(content, result);
        }

    }
}